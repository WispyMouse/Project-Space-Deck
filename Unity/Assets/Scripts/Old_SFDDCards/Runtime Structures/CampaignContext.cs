namespace SFDDCards
{
    using SFDDCards.Evaluation.Actual;
    using SFDDCards.ImportModels;
    using SFDDCards.ScriptingTokens;
    using SFDDCards.ScriptingTokens.EvaluatableValues;
    using SpaceDeck.GameState.Execution;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Models.Databases;
    using SpaceDeck.Models.Instances;
    using SpaceDeck.Utility.Wellknown;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using UnityEngine;

    public class CampaignContext
    {
        public enum GameplayCampaignState
        {
            NotStarted = 0,
            ClearedRoom = 1,
            InCombat = 2,
            Defeat = 3,
            EnteringRoom = 4,
            NonCombatEncounter = 5,
            MakingRouteChoice = 6,
            Victory = 7
        }

        public enum NonCombatEncounterStatus
        {
            NotInNonCombatEncounter = 0,
            AllowedToLeave = 1,
            NotAllowedToLeave = 2
        }

        public readonly GameState GameplayState = new GameState();

        [Obsolete("Should transition to " + nameof(_CampaignDeck))]
        public readonly Deck CampaignDeck;
        public IReadOnlyList<CardInstance> _CampaignDeck => this.GameplayState.CardsInDeck;
        public CombatContext CurrentCombatContext { get; private set; } = null;
        [Obsolete("Transition to " + nameof(_CurrentEncounter))]
        public EvaluatedEncounter CurrentEncounter { get; private set; } = null;
        public EncounterState _CurrentEncounter => this.GameplayState.CurrentEncounterState;

        public readonly Player CampaignPlayer;
        public readonly Entity _CampaignPlayer;

        public CampaignRoute OnRoute { get; private set; } = null;
        public int CampaignRouteNodeIndex { get; private set; } = -1;

        public GameplayCampaignState CurrentGameplayCampaignState { get; private set; } = GameplayCampaignState.NotStarted;
        public NonCombatEncounterStatus CurrentNonCombatEncounterStatus { get; private set; } = NonCombatEncounterStatus.NotInNonCombatEncounter;

        private readonly Dictionary<IReactionWindowReactor, HashSet<ReactionWindowSubscription>> ReactorsToSubscriptions = new Dictionary<IReactionWindowReactor, HashSet<ReactionWindowSubscription>>();
        private readonly Dictionary<string, List<ReactionWindowSubscription>> WindowsToReactors = new Dictionary<string, List<ReactionWindowSubscription>>();

        [Obsolete("Should transition to " + nameof(_CurrencyCounts))]
        public Dictionary<CurrencyImport, int> CurrencyCounts = new Dictionary<CurrencyImport, int>();
        public Dictionary<Currency, int> _CurrencyCounts = new Dictionary<Currency, int>();

        [Obsolete("Should transition to " + nameof(_PendingRewards))]
        public Reward PendingRewards { get; set; } = null;
        public SpaceDeck.Models.Instances.RewardInstance _PendingRewards { get; set; } = null;

        public CampaignContext(CampaignRoute onRoute)
        {
            // New
            this._CampaignPlayer = new Entity();
            this._CampaignPlayer.Qualities.SetNumericQuality(WellknownQualities.Faction, WellknownFactions.Player);
            this._CampaignPlayer.Qualities.SetNumericQuality(WellknownQualities.MaximumHealth, onRoute.BasedOn.StartingMaximumHealth);
            this._CampaignPlayer.Qualities.SetNumericQuality(WellknownQualities.Health, onRoute.BasedOn.StartingMaximumHealth);
            this.GameplayState.AddPersistentEntity(this._CampaignPlayer);

            // Old
            this.CampaignPlayer = new Player(onRoute.BasedOn.StartingMaximumHealth);

            this.OnRoute = onRoute;

            this.CampaignDeck = new Deck(this);
            foreach (string startingCard in onRoute.BasedOn.StartingDeck)
            {
                this.CampaignDeck.AddCardToDeck(CardDatabase.GetModel(startingCard));
                this.GameplayState.AddCard(SpaceDeck.Models.Databases.CardDatabase.GetInstance(startingCard), WellknownZones.Campaign);
            }
        }

        [Obsolete("Mutation of game state should be done through a " + nameof(IGameStateMutator))]
        public void AddCardToDeck(Card toAdd)
        {
            this.CampaignDeck.AddCardToDeck(toAdd);

            GlobalUpdateUX.UpdateUXEvent?.Invoke(this);
        }

        [Obsolete("Mutation of game state should be done through a " + nameof(IGameStateMutator))]
        public void RemoveCardFromDeck(Card toRemove)
        {
            this.CampaignDeck.RemoveCardFromDeck(toRemove);

            if (this.CurrentCombatContext != null)
            {
                this.CurrentCombatContext.PlayerCombatDeck.RemoveCardFromAllZones(toRemove);
            }

            GlobalUpdateUX.UpdateUXEvent?.Invoke(this);
        }

        public void LeaveCurrentEncounter()
        {
            if (this.CurrentCombatContext != null && this.CurrentCombatContext.BasedOnEncounter != null)
            {
                this.PendingRewards = this.CurrentCombatContext.Rewards;
            }

            this.CurrentCombatContext = null;
            this.CurrentEncounter = null;
            this.GameplayState.CurrentEncounterState = null;

            GlobalUpdateUX.UpdateUXEvent.Invoke(this);
        }

        [Obsolete("Mutation of game state should be done through a " + nameof(IGameStateMutator))]
        public void StartNextRoomFromEncounter(EvaluatedEncounter basedOn)
        {
            this.LeaveCurrentEncounter();
            this.CurrentEncounter = basedOn;

            if (basedOn.BasedOn.IsShopEncounter)
            {
                this.SetCampaignState(GameplayCampaignState.NonCombatEncounter, NonCombatEncounterStatus.AllowedToLeave);
                return;
            }
            else if (basedOn.BasedOn.EncounterScripts != null && basedOn.BasedOn.EncounterScripts.Count > 0)
            {
                this.SetCampaignState(GameplayCampaignState.NonCombatEncounter, NonCombatEncounterStatus.NotAllowedToLeave);
                return;
            }

            this.CurrentCombatContext = new CombatContext(this, basedOn);
            this.SetCampaignState(GameplayCampaignState.InCombat);
        }

        public void SetCampaignState(GameplayCampaignState toState, NonCombatEncounterStatus nonCombatState = NonCombatEncounterStatus.NotInNonCombatEncounter)
        {
            this.CurrentGameplayCampaignState = toState;
            this.CurrentNonCombatEncounterStatus = nonCombatState;

            if (toState != GameplayCampaignState.InCombat)
            {
                this.ClearCombatPersistenceStatuses();
            }

            if (toState == GameplayCampaignState.ClearedRoom && this.CurrentEncounter != null && this.CurrentCombatContext.Enemies.Count == 0)
            {
                this.LeaveCurrentEncounter();
            }

            if (toState == GameplayCampaignState.MakingRouteChoice)
            {
                this.CampaignRouteNodeIndex++;

                if (this.OnRoute != null && this.CampaignRouteNodeIndex >= this.OnRoute.Nodes.Count)
                {
                    this.SetCampaignState(GameplayCampaignState.Victory);
                }
            }

            GlobalUpdateUX.UpdateUXEvent?.Invoke(this);
        }

        public void MakeChoiceNodeDecision(ChoiceNodeOption chosen)
        {
            chosen.WasSelected = true;
            this.CurrentEncounter = chosen.WillEncounter;
            this.StartNextRoomFromEncounter(chosen.WillEncounter);
            GlobalUpdateUX.UpdateUXEvent?.Invoke(this);
        }

        public ChoiceNode GetCampaignCurrentNode()
        {
            if (this.OnRoute == null || this.OnRoute.Nodes.Count <= this.CampaignRouteNodeIndex)
            {
                return null;
            }

            return this.OnRoute.Nodes[this.CampaignRouteNodeIndex];
        }

        public void ClearCombatPersistenceStatuses()
        {
            if (this.CampaignPlayer == null)
            {
                return;
            }

            foreach (AppliedStatusEffect effect in new List<AppliedStatusEffect>(this.CampaignPlayer.AppliedStatusEffects))
            {
                if (effect.BasedOnStatusEffect.Persistence == ImportModels.StatusEffectImport.StatusEffectPersistence.Combat)
                {
                    this.CampaignPlayer.AppliedStatusEffects.Remove(effect);
                }
            }
        }

        public void CheckAndApplyReactionWindow(ReactionWindowContext context)
        {
            if (!this.TryGetReactionWindowSequenceEvents(context, out List<GameplaySequenceEvent> eventsThatWouldFollow))
            {
                return;
            }

            GlobalSequenceEventHolder.PushSequencesToTop(this, eventsThatWouldFollow.ToArray());
        }

        public bool TryGetReactionWindowSequenceEvents(ReactionWindowContext context, out List<GameplaySequenceEvent> eventsThatWouldFollow)
        {
            eventsThatWouldFollow = null;

            if (this.WindowsToReactors.TryGetValue(context.TimingWindowId, out List<ReactionWindowSubscription> reactors))
            {
                foreach (ReactionWindowSubscription reactor in reactors)
                {
                    if (reactor != null && reactor.ShouldApply(context) && reactor.Reactor.TryGetReactionEvents(this, context, out List<WindowResponse> responses))
                    {
                        if (eventsThatWouldFollow == null)
                        {
                            eventsThatWouldFollow = new List<GameplaySequenceEvent>();
                        }

                        foreach (WindowResponse response in responses)
                        {
                            eventsThatWouldFollow.Add(new GameplaySequenceEvent(() => this.StatusEffectHappeningProc(new StatusEffectHappening(response))));
                        }
                    }
                }
            }

            if (eventsThatWouldFollow == null)
            {
                return false;
            }

            return true;
        }

        [Obsolete("Mutation of game state should be done through a " + nameof(IGameStateMutator))]
        public void SubscribeToReactionWindow(IReactionWindowReactor reactor, ReactionWindowSubscription subscription)
        {
            if (!this.WindowsToReactors.TryGetValue(subscription.ReactionWindowId.ToLower(), out List<ReactionWindowSubscription> reactorsList))
            {
                reactorsList = new List<ReactionWindowSubscription>();
                this.WindowsToReactors.Add(subscription.ReactionWindowId.ToLower(), reactorsList);
            }

            reactorsList.Add(subscription);

            if (!this.ReactorsToSubscriptions.TryGetValue(reactor, out HashSet<ReactionWindowSubscription> subscriptions))
            {
                subscriptions = new HashSet<ReactionWindowSubscription>();
                this.ReactorsToSubscriptions.Add(reactor, subscriptions);
            }

            subscriptions.Add(subscription);
        }

        [Obsolete("Mutation of game state should be done through a " + nameof(IGameStateMutator))]
        public void UnsubscribeReactor(IReactionWindowReactor reactor)
        {
            if (this.ReactorsToSubscriptions.TryGetValue(reactor, out HashSet<ReactionWindowSubscription> reactions))
            {
                foreach (ReactionWindowSubscription reactionWindow in reactions)
                {
                    this.WindowsToReactors[reactionWindow.ReactionWindowId].Remove(reactionWindow);
                }
                this.ReactorsToSubscriptions.Remove(reactor);
            }
        }

        [Obsolete("Mutation of game state should be done through a " + nameof(IGameStateMutator))]
        public void StatusEffectHappeningProc(StatusEffectHappening happening)
        {
            ICombatantTarget originalTarget = happening.Context.CombatantTarget;

            GamestateDelta delta = ScriptTokenEvaluator.CalculateRealizedDeltaEvaluation(happening, this, happening.OwnedStatusEffect?.Owner, originalTarget, happening.Context);
            GlobalUpdateUX.LogTextEvent.Invoke(EffectDescriberDatabase.DescribeResolvedEffect(delta), GlobalUpdateUX.LogType.GameEvent);
            delta.ApplyDelta(this);

            this.CheckAllStateEffectsAndKnockouts();
        }

        [Obsolete("Mutation of game state should be done through a " + nameof(IGameStateMutator))]
        public void IngestStatusEffectHappening(ReactionWindowContext reactionWindow, WindowResponse response)
        {
            GlobalSequenceEventHolder.PushSequencesToTop(reactionWindow.CampaignContext, response);
        }

        [Obsolete("Mutation of game state should be done through a " + nameof(IGameStateMutator))]
        public void CheckAllStateEffectsAndKnockouts()
        {
            if (this.CampaignPlayer.CurrentHealth <= 0)
            {
                this.PlayerDefeat();
                return;
            }

            if (this.CurrentCombatContext != null)
            {
                List<Enemy> enemies = new List<Enemy>(this.CurrentCombatContext.Enemies);
                foreach (Enemy curEnemy in enemies)
                {
                    if (curEnemy.ShouldBecomeDefeated && !curEnemy.DefeatHasBeenSignaled)
                    {
                        this.CurrentCombatContext.RemoveEnemy(curEnemy);
                    }
                }
            }

            if (this.CurrentNonCombatEncounterStatus == CampaignContext.NonCombatEncounterStatus.NotInNonCombatEncounter && this.CurrentCombatContext.Enemies.Count == 0)
            {
                GlobalUpdateUX.LogTextEvent.Invoke($"There are no more enemies!", GlobalUpdateUX.LogType.GameEvent);
                this.SetCampaignState(CampaignContext.GameplayCampaignState.ClearedRoom);
                return;
            }

            GlobalUpdateUX.UpdateUXEvent?.Invoke(this);
        }

        [Obsolete("Mutation of game state should be done through a " + nameof(IGameStateMutator))]
        private void PlayerDefeat()
        {
            foreach (AppliedStatusEffect effect in this.CampaignPlayer.AppliedStatusEffects)
            {
                this.UnsubscribeReactor(effect);
            }

            GlobalUpdateUX.LogTextEvent.Invoke($"The player has run out of health! This run is over.", GlobalUpdateUX.LogType.GameEvent);
            this.SetCampaignState(CampaignContext.GameplayCampaignState.Defeat);
            this.UnsubscribeReactor(this.CampaignPlayer);

            GlobalSequenceEventHolder.StopAllSequences();
        }

        [Obsolete("Mutation of game state should be done through a " + nameof(IGameStateMutator))]
        public int _GetCurrencyCount(Currency toGet)
        {
            if (this._CurrencyCounts.TryGetValue(toGet, out int value))
            {
                return value;
            }

            return 0;
        }

        [Obsolete("Should transition to " + nameof(_GetCurrencyCount))]
        public int GetCurrencyCount(CurrencyImport toGet)
        {
            if (this.CurrencyCounts.TryGetValue(toGet, out int value))
            {
                return value;
            }

            return 0;
        }

        [Obsolete("Mutation of game state should be done through a " + nameof(IGameStateMutator))]
        public void _ModCurrency(Currency toAward, int amount)
        {
            if (this._CurrencyCounts.TryGetValue(toAward, out int existingValue))
            {
                this._CurrencyCounts[toAward] = Mathf.Max(0, existingValue + amount);
            }
            else
            {
                this._CurrencyCounts.Add(toAward, Mathf.Max(0, amount));
            }

            GlobalUpdateUX.UpdateUXEvent.Invoke(this);
        }

        [Obsolete("Should transition to " + nameof(_ModCurrency))]
        public void ModCurrency(SFDDCards.ImportModels.CurrencyImport toAward, int amount)
        {
            if (this.CurrencyCounts.TryGetValue(toAward, out int existingValue))
            {
                this.CurrencyCounts[toAward] = Mathf.Max(0, existingValue + amount);
            }
            else
            {
                this.CurrencyCounts.Add(toAward, Mathf.Max(0, amount));
            }

            GlobalUpdateUX.UpdateUXEvent.Invoke(this);
        }

        [Obsolete("Mutation of game state should be done through a " + nameof(IGameStateMutator))]
        public void _SetCurrency(Currency toSet, int amount)
        {
            if (this._CurrencyCounts.TryGetValue(toSet, out int existingValue))
            {
                this._CurrencyCounts[toSet] = Mathf.Max(0, amount);
            }
            else
            {
                this._CurrencyCounts.Add(toSet, Mathf.Max(0, amount));
            }

            GlobalUpdateUX.UpdateUXEvent.Invoke(this);
        }

        [Obsolete("Should transition to " + nameof(_SetCurrency))]
        public void SetCurrency(CurrencyImport toSet, int amount)
        {
            if (this.CurrencyCounts.TryGetValue(toSet, out int existingValue))
            {
                this.CurrencyCounts[toSet] = Mathf.Max(0, amount);
            }
            else
            {
                this.CurrencyCounts.Add(toSet, Mathf.Max(0, amount));
            }

            GlobalUpdateUX.UpdateUXEvent.Invoke(this);
        }

        public void PurchaseShopItem(ShopEntry toBuy)
        {
            bool canAfford = this.CanAfford(toBuy.Costs);

            if (!canAfford)
            {
                GlobalUpdateUX.LogTextEvent.Invoke($"Cannot afford the cost of this item, or could not evaluate all its costs.", GlobalUpdateUX.LogType.UserError);
                return;
            }

            this.Gain(toBuy);

            foreach (ShopCost cost in toBuy.Costs)
            {
                if (!cost.Amount.TryEvaluateValue(this, null, out int shopCostAmount))
                {
                    GlobalUpdateUX.LogTextEvent.Invoke($"The cost of a shop item could not be evaluated.", GlobalUpdateUX.LogType.RuntimeError);
                }

                this.ModCurrency(cost.CurrencyType, -shopCostAmount);
            }
        }

        public void _PurchaseShopItem(IShopEntry toBuy)
        {
            bool canAfford = this.GameplayState.CanAfford(toBuy.Costs);

            if (!canAfford)
            {
                GlobalUpdateUX.LogTextEvent.Invoke($"Cannot afford the cost of this item, or could not evaluate all its costs.", GlobalUpdateUX.LogType.UserError);
                return;
            }

            this.Gain(toBuy);

            foreach (ShopCost cost in toBuy.Costs)
            {
                if (!cost.Amount.TryEvaluateValue(this, null, out int shopCostAmount))
                {
                    GlobalUpdateUX.LogTextEvent.Invoke($"The cost of a shop item could not be evaluated.", GlobalUpdateUX.LogType.RuntimeError);
                }

                this.ModCurrency(cost.CurrencyType, -shopCostAmount);
            }
        }

        public bool CanAfford(List<ShopCost> costs)
        {
            foreach (ShopCost cost in costs)
            {
                if (!cost.Amount.TryEvaluateValue(this, null, out int costAmount))
                {
                    return false;
                }

                int amountInPossession = GetCurrencyCount(cost.CurrencyType);

                if (amountInPossession < costAmount)
                {
                    return false;
                }
            }

            return true;
        }

        public void Gain(IGainable toGain)
        {
            if (!toGain.GainedAmount.TryEvaluateValue(this, null, out int gainedAmount))
            {
                GlobalUpdateUX.LogTextEvent.Invoke($"Failed to evaluate gain amount for gainable. Could not gain.", GlobalUpdateUX.LogType.RuntimeError);
                return;
            }

            if (toGain.GainedCard != null)
            {
                for (int ii = 0; ii < gainedAmount; ii++)
                {
                    this.CampaignDeck.AddCardToDeck(toGain.GainedCard);
                }
            }
            else if (toGain.GainedEffect != null)
            {
                GlobalSequenceEventHolder.PushSequenceToTop(new GameplaySequenceEvent(() =>
                {
                    this.CampaignPlayer.ApplyDelta(
                        this,
                        null,
                        ScriptTokenEvaluator.GetDeltaFromTokens($"[SETTARGET:SELF][APPLYSTATUSEFFECTSTACKS: {gainedAmount} {toGain.GainedEffect.Id}]",
                        this,
                        null,
                        this.CampaignPlayer,
                        this.CampaignPlayer)
                        .DeltaEntries[0]);

                    GlobalUpdateUX.UpdateUXEvent?.Invoke(this);
                }));
            }
            else if (toGain.GainedCurrency != null)
            {
                this._ModCurrency(toGain.GainedCurrency, gainedAmount);
            }

            GlobalUpdateUX.UpdateUXEvent.Invoke(this);
        }

        public void Gain(IShopEntry toGain)
        {
            throw new System.NotImplementedException();
        }

        public List<ShopCost> GetPriceForItem(IGainable toShopFor)
        {
            Dictionary<Currency, IEvaluatableValue<int>> costs = new Dictionary<Currency, IEvaluatableValue<int>>();

            foreach (CostEvaluationModifier modifier in this.OnRoute.BasedOn.CostModifiers)
            {
                if (!GainableMatchesTags(toShopFor, modifier.TagMatch))
                {
                    continue;
                }

                Currency referencedCurrency = CurrencyDatabase.Get(modifier.Currency);
                int valueToReplace = 0;

                if (costs.TryGetValue(referencedCurrency, out IEvaluatableValue<int> currentValue))
                {
                    if (!currentValue.TryEvaluateValue(this, null, out valueToReplace))
                    {
                        GlobalUpdateUX.LogTextEvent.Invoke($"Failed to evaluate existing cost from earlier rule.", GlobalUpdateUX.LogType.RuntimeError);
                        continue;
                    }
                }

                string replacedValueString = modifier.EvaluationScript.ToLower().Replace("value", valueToReplace.ToString());
                if (!BaseScriptingToken.TryGetIntegerEvaluatableFromString(replacedValueString, out IEvaluatableValue<int> newValue))
                {
                    GlobalUpdateUX.LogTextEvent.Invoke($"Failed to evaluate new rule.", GlobalUpdateUX.LogType.RuntimeError);
                    continue;
                }

                if (costs.ContainsKey(referencedCurrency))
                {
                    costs[referencedCurrency] = newValue;
                }
                else
                {
                    costs.Add(referencedCurrency, newValue);
                }
            }

            List<ShopCost> costsAsShopCost = new List<ShopCost>();
            foreach (Currency key in costs.Keys)
            {
                costsAsShopCost.Add(new ShopCost() { Amount = costs[key], _CurrencyType = key });
            }

            return costsAsShopCost;
        }

        public bool GainableMatchesTags(IGainable toMatch, IEnumerable<string> tags)
        {
            if (toMatch.GainedCard != null)
            {
                return toMatch.GainedCard.BasedOn.MeetsAllTags(tags);
            }
            else if (toMatch.GainedEffect != null)
            {
                return toMatch.GainedEffect.MeetsAllTags(tags);
            }
            else if (toMatch.GainedCurrency != null)
            {
                // TODO: Currency tags?
                return true;
            }

            return false;
        }

        public bool RequirementsAreMet(string requirementScript)
        {
            if (string.IsNullOrEmpty(requirementScript))
            {
                return true;
            }

            RequiresComparisonScriptingToken requiresComparisonBaseToken = new RequiresComparisonScriptingToken();
            if (requiresComparisonBaseToken.GetTokenIfMatch(requirementScript, out IScriptingToken match) && match is RequiresComparisonScriptingToken requiresComparison)
            {
                return requiresComparison.MeetsRequirement(null, this);
            }

            return true;
        }
    }
}