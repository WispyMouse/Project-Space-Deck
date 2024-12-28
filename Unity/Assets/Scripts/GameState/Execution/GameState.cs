namespace SpaceDeck.GameState.Execution
{
    using SpaceDeck.GameState.Changes;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Models.Instances;
    using SpaceDeck.Tokenization.Minimum.Context;
    using SpaceDeck.Tokenization.Minimum.Questions;
    using SpaceDeck.Utility.Minimum;
    using SpaceDeck.Utility.Wellknown;
    using System;
    using System.Collections;
    using System.Collections.Generic;

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

        }

        public void SetNumericQuality(IHaveQualities entity, LowercaseString index, decimal toValue)
        {
            entity.Qualities.SetNumericQuality(index, toValue);
        }

        public decimal GetNumericQuality(IHaveQualities entity, LowercaseString index, decimal defaultValue = 0)
        {
            return entity.Qualities.GetNumericQuality(index, defaultValue);
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
            this.TriggerAndStack(new GameStateEventTrigger(Utility.Wellknown.WellknownGameStateEvents.FactionTurnStarted));
        }

        public void StartEntityTurn(Entity toStart)
        {
            this.TriggerAndStack(new GameStateEventTrigger(Utility.Wellknown.WellknownGameStateEvents.EntityTurnStarted));
        }

        public void EndCurrentEntityTurn()
        {
            this.TriggerAndStack(new GameStateEventTrigger(Utility.Wellknown.WellknownGameStateEvents.EntityTurnEnded));
        }

        public void EndCurrentFactionTurn()
        {
            this.TriggerAndStack(new GameStateEventTrigger(Utility.Wellknown.WellknownGameStateEvents.FactionTurnEnded));
        }

        public void TriggerAndStack(GameStateEventTrigger trigger)
        {
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
                this.PendingResolves.Push(new ResolveTriggeredEvent(effect, trigger));
            }

            // Then put on top of the stack each rule in order of application
            // This way, rules always resolve before the triggered abilities consider triggering
            List<GameStateChange> appliedRules = RuleReference.GetAppliedRules(this, trigger);
            foreach (GameStateChange change in appliedRules)
            {
                this.PendingResolves.Push(change);
            }
            
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
            return new QuestionAnsweringContext(this);
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

            // Put the movement to discard on the stack first
            this.PendingResolves.Push(new MoveCard(this.CurrentlyConsideredPlayedCard, WellknownZones.Discard));

            LinkedCardInstance linkedCard = this.CurrentlyConsideredPlayedCard as LinkedCardInstance;
            this.CurrentlyConsideredPlayedCard = null;

            // If the token has the appropriate information, execute it
            if (linkedCard != null)
            {
                if (linkedCard.Prototype.LinkedTokens.HasValue && GameStateDeltaMaker.TryCreateDelta(linkedCard.Prototype.LinkedTokens.Value, answers, this, out GameStateDelta delta))
                {
                    GameStateDeltaApplier.ApplyGameStateDelta(this, delta);
                }
            }
            
            PendingResolveExecutor.ResolveAll(this);

            return true;
        }

        public bool TryExecuteCurrentCard()
        {
            return TryExecuteCurrentCard(null);
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
    }
}