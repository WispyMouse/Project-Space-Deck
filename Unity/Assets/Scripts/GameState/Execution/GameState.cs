namespace SpaceDeck.GameState.Execution
{
    using SpaceDeck.GameState.Changes;
    using SpaceDeck.GameState.Deltas;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Models.Databases;
    using SpaceDeck.Models.Instances;
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.Tokenization.Minimum.Context;
    using SpaceDeck.Tokenization.Minimum.Questions;
    using SpaceDeck.Utility.Logging;
    using SpaceDeck.Utility.Minimum;
    using SpaceDeck.Utility.Wellknown;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using static SpaceDeck.GameState.Minimum.GameStateEventTrigger;

    /// <summary>
    /// Describes the state of a game.
    /// 
    /// This is meant to be entirely hollistic, holding information
    /// about every character, decision, card, deck, battle, etc.
    /// 
    /// This is not necessarily the current game's state. This object
    /// can be cloned, allowing for theoretical branches of the game's
    /// state in order to show certain properties and values that might
    /// exist after something is executed.
    /// </summary>
    public class GameState : IGameStateMutator, ICardPlayer
    {
        public EncounterState CurrentEncounterState = null;
        public readonly List<Entity> PersistentEntities = new List<Entity>();
        public readonly PendingResolveStack PendingResolves = new PendingResolveStack();
        public readonly List<CardInstance> CardsInDeck = new List<CardInstance>();
        public readonly Dictionary<Currency, int> Currencies = new Dictionary<Currency, int>();
        public readonly Dictionary<Element, int> Elements = new Dictionary<Element, int>();
        public readonly Route BasedOnRoute;
        public int RouteIndex = -1;
        public List<PickReward> PendingRewards;

        public EntityTurnTakerCalculator EntityTurnTakerCalculator { get; set; }
        public FactionTurnTakerCalculator FactionTurnTakerCalculator { get; set; }

        public CardInstance CurrentlyConsideredPlayedCard { get; set; }

        public List<Entity> AllEntities
        {
            get
            {
                // TODO: This is pretty horrible (I wrote it!)! It has a lot of allocating the
                // same list repeatedly. We should revisit this soon.
                List<Entity> allEntities = new List<Entity>(PersistentEntities);

                if (this.CurrentEncounterState != null)
                {
                    allEntities.AddRange(this.CurrentEncounterState.EncounterEntities);
                }
                return allEntities;
            }
        }

        public GameState()
        {
            this.BasedOnRoute = null;
        }

        public GameState(Route basedOnRoute)
        {
            this.BasedOnRoute = basedOnRoute;
        }

        public void SetNumericQuality(IHaveQualities entity, LowercaseString index, decimal toValue)
        {
            entity.Qualities.SetNumericQuality(index, toValue);
        }

        public decimal GetNumericQuality(IHaveQualities entity, LowercaseString index, decimal defaultValue = 0)
        {
            return entity.Qualities.GetNumericQuality(index, defaultValue);
        }

        public void ModCurrency(Currency toMod, int modAmount)
        {
            if (this.Currencies.TryGetValue(toMod, out int currentValue))
            {
                this.Currencies[toMod] = currentValue + modAmount;
            }
            else
            {
                this.Currencies.Add(toMod, modAmount);
            }
        }

        public int GetCurrency(Currency toGet)
        {
            if (this.Currencies.TryGetValue(toGet, out int currentValue))
            {
                return currentValue;
            }

            this.Currencies.Add(toGet, 0);
            return this.GetCurrency(toGet);
        }

        public void RemoveEntity(Entity entity)
        {
            this.PersistentEntities.Remove(entity);
            this.CurrentEncounterState.EncounterEntities.Remove(entity);
        }

        public bool EntityIsAlive(Entity entity)
        {
            return this.PersistentEntities.Contains(entity) || (this.CurrentEncounterState?.EncounterEntities.Contains(entity) ?? false);
        }

        public IReadOnlyList<Entity> GetAllEntities()
        {
            return this.AllEntities;
        }

        public void AddEncounterEntity(Entity toAdd)
        {
            this.CurrentEncounterState?.EncounterEntities.Add(toAdd);
        }

        public void AddPersistentEntity(Entity toAdd)
        {
            this.PersistentEntities.Add(toAdd);
        }

        public void StartFactionTurn(decimal factionId)
        {
            this.TriggerAndStack(new GameStateEventTrigger(Utility.Wellknown.WellknownGameStateEvents.FactionTurnStarted, new ActionExecutor((IGameStateMutator mutator) => { mutator.FactionTurnTakerCalculator.SetCurrentTurnTaker(factionId); })));
        }

        public void StartEntityTurn(Entity toStart)
        {
            this.TriggerAndStack(new GameStateEventTrigger(Utility.Wellknown.WellknownGameStateEvents.EntityTurnStarted, 
                new ActionExecutor((IGameStateMutator mutator) => { mutator.EntityTurnTakerCalculator.SetCurrentTurnTaker(toStart); })));
        }

        public void EndCurrentEntityTurn()
        {
            if (this.EntityTurnTakerCalculator.TryGetCurrentEntityTurn(this, out Entity currentTurn))
            {
                this.TriggerAndStack(new GameStateEventTrigger(Utility.Wellknown.WellknownGameStateEvents.EntityTurnEnded, currentTurn));
            }
        }

        public void EndCurrentFactionTurn()
        {
            this.TriggerAndStack(new GameStateEventTrigger(Utility.Wellknown.WellknownGameStateEvents.FactionTurnEnded));
        }

        public void TriggerAndStack(GameStateEventTrigger trigger)
        {
            // The order of resolution should be:
            // - "Before" triggered effects, that are responding to an event being triggered
            // - If there is a GameStateChange to apply, apply it here
            // - "After" triggered effects
            // So that means we stack "after" first
            this.PushResolve(new TriggerAndResolve(trigger, TriggerDirection.After));

            if (trigger.BasedOnGameStateChange != null)
            {
                this.PushResolve(trigger.BasedOnGameStateChange);
            }

            this.PushResolve(new TriggerAndResolve(trigger, TriggerDirection.Before));
        }

        public IReadOnlyList<IResolve> GetTriggers(GameStateEventTrigger trigger, TriggerDirection direction)
        {
            List<IResolve> triggeredResolves = new List<IResolve>();

            // First put on to the stack an instruction to resolve each status effect
            // We can grab all status effects with matching ids for trigger events
            // and then as they're resolving, can determine if anything should actually happen,
            // including whether or not the triggered ability actually triggers
            List<AppliedStatusEffect> possiblyTriggeredStatusEffects = new List<AppliedStatusEffect>();
            foreach (AppliedStatusEffect effect in this.GetAllStatusEffects())
            {
                if (effect.TriggerOnEventIds.Contains(trigger.EventId))
                {
                    possiblyTriggeredStatusEffects.Add(effect);
                }
            }
            possiblyTriggeredStatusEffects.Sort((a, b) => a.StatusEffectPriorityOrder.CompareTo(b.StatusEffectPriorityOrder));
            foreach (AppliedStatusEffect effect in possiblyTriggeredStatusEffects)
            {
                triggeredResolves.Add(new ResolveTriggeredEvent(effect, trigger, direction));
            }

            // Then put on top of the stack each rule in order of application
            // This way, rules always resolve before the triggered abilities consider triggering
            List<GameStateChange> appliedRules = RuleReference.GetAppliedRules(this, direction, trigger);
            foreach (GameStateChange change in appliedRules)
            {
                triggeredResolves.Add(change);
            }

            return triggeredResolves;
        }

        public bool TryGetNextResolve(out IResolve currentResolve)
        {
            if (!this.PendingResolves.TryGetNextResolve(out currentResolve))
            {
                return false;
            }

            return true;
        }

        public void StartEncounter(EncounterState encounter)
        {
            this.CurrentEncounterState = encounter;
            this.TriggerAndStack(new GameStateEventTrigger(WellknownGameStateEvents.EncounterStart));
        }

        public void MoveCard(CardInstance card, LowercaseString zone)
        {
            this.CurrentEncounterState.MoveCard(card, zone);
        }

        public IReadOnlyList<CardInstance> GetCardsInZone(LowercaseString zone)
        {
            if (zone == WellknownZones.Campaign)
            {
                return this.CardsInDeck;
            }

            if (this.CurrentEncounterState != null && this.CurrentEncounterState.ZonesWithCards.ContainsKey(zone))
            {
                return this.CurrentEncounterState.ZonesWithCards[zone];
            }

            return Array.Empty<CardInstance>();
        }

        public void ShuffleDeck()
        {
            if (this.CurrentEncounterState == null)
            {
                return;
            }

            if (!this.CurrentEncounterState.ZonesWithCards.ContainsKey(WellknownZones.Deck))
            {
                return;
            }

            List<CardInstance> cardsToShuffle = new List<CardInstance>(this.CurrentEncounterState.ZonesWithCards[WellknownZones.Deck]);
            this.CurrentEncounterState.ZonesWithCards[WellknownZones.Deck].Clear();

            while (cardsToShuffle.Count > 0)
            {
                int randomIndex = new Random().Next(cardsToShuffle.Count);
                this.CurrentEncounterState.ZonesWithCards[WellknownZones.Deck].Add(cardsToShuffle[randomIndex]);
                cardsToShuffle.RemoveAt(randomIndex);
            }
        }

        public void AddCard(CardInstance card, LowercaseString zone)
        {
            if (this.CurrentEncounterState == null)
            {
                if (zone == WellknownZones.Campaign)
                {
                    this.CardsInDeck.Add(card);
                }

                return;
            }

            if (!this.CurrentEncounterState.ZonesWithCards.ContainsKey(zone))
            {
                this.CurrentEncounterState.ZonesWithCards.Add(zone, new List<CardInstance>());
            }

            this.CurrentEncounterState.ZonesWithCards[zone].Add(card);
            this.CurrentEncounterState.CardsInZones.Add(card, zone);
        }

        public QuestionAnsweringContext StartConsideringPlayingCard(CardInstance toPlay)
        {
            this.CurrentlyConsideredPlayedCard = toPlay;
            return new QuestionAnsweringContext(this, this.GetPlayerEntity(), toPlay);
        }

        public void EntityPerformsAction(Entity toAct, LinkedToken toPerform)
        {
            this.EntityPerformsAction(toAct, toPerform, new ExecutionAnswerSet(toAct));
        }

        public void EntityPerformsAction(Entity toAct, LinkedToken toPerform, ExecutionAnswerSet answers)
        {
            QuestionAnsweringContext questionAnsweringContext = new QuestionAnsweringContext(this, toAct);
            answers.SetDefaultAnswers(toPerform.Questions, questionAnsweringContext);

            if (!GameStateDeltaMaker.TryCreateDelta(new LinkedTokenList(toPerform), answers, this, out GameStateDelta delta))
            {
                Logging.DebugLog(WellknownLoggingLevels.Error,
                    WellknownLoggingCategories.TryCreateDelta,
                    $"Failed to create delta for entity performing action.");
                return;
            }

            GameStateDeltaApplier.ApplyGameStateDelta(this, delta);
            PendingResolveExecutor.ResolveAll(this);
        }

        public void EntityPerformsActions(Entity toAct, LinkedTokenList toPerform, ExecutionAnswerSet answers)
        {
            QuestionAnsweringContext questionAnsweringContext = new QuestionAnsweringContext(this, toAct);
            answers.SetDefaultAnswers(toPerform.GetQuestions(), questionAnsweringContext);

            if (!GameStateDeltaMaker.TryCreateDelta(toPerform, answers, this, out GameStateDelta delta))
            {
                Logging.DebugLog(WellknownLoggingLevels.Error,
                    WellknownLoggingCategories.TryCreateDelta,
                    $"Failed to create delta for entity performing action.");
                return;
            }

            GameStateDeltaApplier.ApplyGameStateDelta(this, delta);
            PendingResolveExecutor.ResolveAll(this);
        }

        public void EntityPlaysLinkedCard(Entity toAct, LinkedCardInstance toPlay, ExecutionAnswerSet answers)
        {
            QuestionAnsweringContext questionAnsweringContext = new QuestionAnsweringContext(this, toAct);
            answers.SetDefaultAnswers(toPlay.Prototype.LinkedTokens.Value.GetQuestions(), questionAnsweringContext);

            if (!GameStateDeltaMaker.TryCreateDelta(toPlay.Prototype.LinkedTokens.Value, answers, this, out GameStateDelta delta, playedCard: toPlay))
            {
                Logging.DebugLog(WellknownLoggingLevels.Error,
                    WellknownLoggingCategories.TryCreateDelta,
                    $"Failed to create delta for entity performing action.");
                return;
            }

            GameStateDeltaApplier.ApplyGameStateDelta(this, delta);
            PendingResolveExecutor.ResolveAll(this);
        }

        public bool TryGetCurrentQuestions(out IReadOnlyList<ExecutionQuestion> questions)
        {
            if (this.CurrentlyConsideredPlayedCard != null)
            {
                if (this.CurrentlyConsideredPlayedCard is LinkedCardInstance linkedCard)
                {
                    if (linkedCard.Prototype == null || !linkedCard.Prototype.LinkedTokens.HasValue)
                    {
                        // TODO: This card isn't properly set up; should log here
                        // But for now, can't get questions from nothing
                        questions = null;
                        return false;
                    }

                    questions = linkedCard.Prototype.LinkedTokens.Value.GetQuestions();
                    return questions.Count > 0;
                }
            }

            // TODO: Things on the resolve stack should be permitted to ask questions too
            // There might be triggered actions that need resolving
            questions = null;
            return false;
        }

        public bool TryExecuteCurrentCard(ExecutionAnswerSet answers)
        {
            IReadOnlyList<ExecutionQuestion> questions;
            if (!TryGetCurrentQuestions(out questions))
            {
                questions = new List<ExecutionQuestion>();
            }

            if (questions.Count > 0)
            {
                if (answers == null)
                {
                    // There are questions but no answers; cannot execute
                    return false;
                }

                foreach (ExecutionQuestion curQuestion in questions)
                {
                    if (!answers.TryGetAnswerForQuestion(curQuestion, out _))
                    {
                        // There's a question without an answer; cannot execute
                        return false;
                    }
                }
            }

            // Store the currently considered card, so we don't have pointer shenanigans
            CardInstance cardInstance = this.CurrentlyConsideredPlayedCard;
            this.CurrentlyConsideredPlayedCard = null;

            GameStateEventTrigger trigger = new GameStateEventTrigger(WellknownGameStateEvents.CardPlayed, cardInstance);
            this.PushResolve(new TriggerAndResolve(trigger, TriggerDirection.After));

            // This is where we should push something to execute the card
            // Executing a card is basically "the player takes an action", right?
            // HACK: Definitely feels wrong to need to unbox cards to play them
            // Need some other solution for getting or using Linked things...
            if (cardInstance is LinkedCardInstance linkedCardInstance && linkedCardInstance.Prototype.LinkedTokens.HasValue)
            {
                this.PushResolve(
                    new ActionExecutor(
                        (IGameStateMutator mutator) => 
                        {
                            this.EntityPlaysLinkedCard(this.GetPlayerEntity(), linkedCardInstance, answers);
                        }
                    ));
            }

            this.PushResolve(new TriggerAndResolve(trigger, TriggerDirection.Before));

            PendingResolveExecutor.ResolveAll(this);

            return true;
        }

        public IReadOnlyList<AppliedStatusEffect> GetAllStatusEffects()
        {
            List<AppliedStatusEffect> statusEffects = new List<AppliedStatusEffect>();

            foreach (Entity curEntity in this.GetAllEntities())
            {
                foreach (AppliedStatusEffect effect in curEntity.AppliedStatusEffects.Values)
                {
                    statusEffects.Add(effect);
                }
            }

            return statusEffects;
        }

        public void PushResolve(IResolve toResolve)
        {
            this.PendingResolves.Push(toResolve);
        }

        public bool CanAfford(IEnumerable<IShopCost> costs)
        {
            throw new NotImplementedException();
        }

        public LowercaseString GetCardZone(CardInstance card)
        {
            if (this.CurrentEncounterState == null)
            {
                if (this.CardsInDeck.Contains(card))
                {
                    return WellknownZones.Campaign;
                }
                else
                {
                    return string.Empty;
                }
            }

            return this.CurrentEncounterState.GetCardZone(card);
        }

        public void PurchaseShopItem(IShopEntry toBuy)
        {
            bool canAfford = this.CanAfford(toBuy.Costs);

            if (!canAfford)
            {
                // TODO: LOG
                return;
            }

            this.Gain(toBuy);

            foreach (IShopCost cost in toBuy.Costs)
            {
                int currencyAmount = cost.GetCost(this);
                this.ModCurrency(cost.CurrencyType, -currencyAmount);
            }
        }

        public void Gain(IShopEntry toGain)
        {
            throw new System.NotImplementedException();
        }

        public void Gain(Reward toGain)
        {
            throw new System.NotImplementedException();
        }

        public ChoiceNode GetCampaignCurrentNode()
        {
            if (this.BasedOnRoute == null || this.BasedOnRoute.Choices.Count <= this.RouteIndex)
            {
                return null;
            }

            return this.BasedOnRoute.Choices[this.RouteIndex];
        }

        public void MakeChoiceNodeDecision(ChoiceNodeOption chosen)
        {
            chosen.WasSelected = true;
            this.StartNextRoomFromEncounter(chosen.WillEncounter);
        }

        public void StartNextRoomFromEncounter(EncounterState basedOn)
        {
            this.CurrentEncounterState = null;
            this.StartEncounter(basedOn);
        }

        public bool StartNextRoomFromCampaign(out ChoiceNode nextChoice)
        {
            this.RouteIndex++;

            if (this.RouteIndex >= this.BasedOnRoute.Choices.Count)
            {
                // TODO: LOG VICTORY
                nextChoice = null;
                return false;
            }

            nextChoice = this.BasedOnRoute.Choices[this.RouteIndex];
            return true;
        }

        public void SetStringQuality(IHaveQualities entity, LowercaseString index, string toValue)
        {
            entity.Qualities.SetStringQuality(index, toValue);
        }

        public string GetStringQuality(IHaveQualities entity, LowercaseString index, string defaultValue = "")
        {
            return entity.Qualities.GetStringQuality(index, defaultValue);
        }

        public void ShuffleDiscardAndDeck()
        {
            foreach (CardInstance cardFromDiscard in new List<CardInstance>(this.GetCardsInZone(WellknownZones.Discard)))
            {
                this.MoveCard(cardFromDiscard, WellknownZones.Deck);
            }
            this.ShuffleDeck();
        }

        public void ModStatusEffectStacks(Entity onEntity, LowercaseString statusEffectId, int modStacks)
        {
            if (!onEntity.AppliedStatusEffects.TryGetValue(statusEffectId, out AppliedStatusEffect effects))
            {
                effects = StatusEffectDatabase.GetInstance(statusEffectId);
                onEntity.AppliedStatusEffects.Add(statusEffectId, effects);
            }

            effects.Qualities.SetNumericQuality(WellknownQualities.Stacks, GetStacks(onEntity, statusEffectId) + modStacks);
        }

        public int GetStacks(Entity onEntity, LowercaseString statusEffectId)
        {
            if (!onEntity.AppliedStatusEffects.TryGetValue(statusEffectId, out AppliedStatusEffect effects))
            {
                return 0;
            }

            return (int)effects.Qualities.GetNumericQuality(WellknownQualities.Stacks, 0);
        }

        public void ModifyElement(Element element, int modAmount)
        {
            if (!this.Elements.ContainsKey(element))
            {
                this.Elements.Add(element, modAmount);
            }
            else
            {
                this.Elements[element] += modAmount;
            }
        }

        public int GetElement(Element element)
        {
            if (this.Elements.TryGetValue(element, out int elementCount))
            {
                return elementCount;
            }

            // This must be an element we should care about, so track it.
            this.Elements.Add(element, 0);

            return 0;
        }

        public void SetIntensity(IChangeWithIntensity intensity, decimal newValue)
        {
            intensity.Intensity = newValue;
        }

        public decimal GetIntensity(IChangeWithIntensity intensity)
        {
            return intensity.Intensity;
        }

        /// <summary>
        /// HACK: For now, use this to point to the player entity.
        /// Ideally we wouldn't make assumption like this in the base code.
        /// </summary>
        public Entity GetPlayerEntity()
        {
            foreach (Entity curEntity in this.PersistentEntities)
            {
                if (this.GetNumericQuality(curEntity, WellknownQualities.Faction) == WellknownFactions.Player)
                {
                    return curEntity;
                }
            }

            return null;
        }
    }
}