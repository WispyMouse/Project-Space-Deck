namespace SpaceDeck.UX
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;
    using System.Text.RegularExpressions;
    using UnityEngine;
    using SFDDCards;
    using SFDDCards.Evaluation.Actual;
    using SFDDCards.ImportModels;
    using SpaceDeck.GameState.Changes.Targets;
    using SpaceDeck.GameState.Execution;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Utility.Wellknown;
    using SpaceDeck.UX;
    using SpaceDeck.UX.AssetLookup;

    public class GameplayUXController : MonoBehaviour
    {
        [SerializeReference]
        private CentralGameStateController CentralGameStateControllerInstance;
        [SerializeReference]
        private AnimationRunnerController AnimationRunnerController;
        [SerializeReference]
        private PlayerStatusEffectUXHolder PlayerStatusEffectUXHolderInstance;
        [SerializeReference]
        private CombatTurnController CombatTurnCounterInstance;
        [SerializeReference]
        private CampaignChooserUX CampaignChooserUXInstance;
        [SerializeReference]
        private CardBrowser CardBrowserUXInstance;
        [SerializeReference]
        private EncounterRepresenterUX EncounterRepresenterUXInstance;

        [SerializeReference]
        private RewardsPanelUX RewardsPanelUXInstance;
        [SerializeReference]
        private ShopUX ShopPanelUXInstance;

        [SerializeReference]
        private PlayerUX PlayerRepresentationPF;
        private PlayerUX PlayerUXInstance { get; set; }

        [SerializeReference]
        private Transform PlayerRepresentationTransform;

        [SerializeReference]
        private PlayerHandRepresenter PlayerHandRepresenter;
        [SerializeReference]
        private EnemyRepresenterUX EnemyRepresenterUX;

        [SerializeReference]
        private GameObject GoNextRoomButton;
        [SerializeReference]
        private GameObject ResetGameButton;
        [SerializeReference]
        private GameObject EndTurnButton;

        [SerializeReference]
        private ChoiceNodeSelectorUX ChoiceSelectorUX;
        [SerializeReference]
        private GameObject CombatUXFolder;
        [SerializeReference]
        private GameObject ChoiceUXFolder;

        [SerializeReference]
        private TMPro.TMP_Text LifeValue;
        [SerializeReference]
        private TMPro.TMP_Text ElementsValue;
        [SerializeReference]
        private TMPro.TMP_Text CurrenciesValue;

        [SerializeReference]
        private TargetableIndicator SingleCombatantTargetableIndicatorPF;
        private List<TargetableIndicator> ActiveIndicators { get; set; } = new List<TargetableIndicator>();
        [SerializeReference]
        private TargetableIndicator NoTargetsIndicator;
        [SerializeReference]
        private TargetableIndicator AllFoeTargetsIndicator;

        [SerializeReference]
        private GameObject AllCardsBrowserButton;

        [SerializeReference]
        private TMPro.TMP_Text Log;

        public DisplayedCardUX CurrentSelectedCard { get; private set; } = null;
        public bool PlayerIsCurrentlyAnimating { get; private set; } = false;

        [Obsolete("These will be obsoleted with new data types.")]
        private CampaignContext.GameplayCampaignState previousCampaignState { get; set; } = CampaignContext.GameplayCampaignState.NotStarted;
        [Obsolete("These will be obsoleted with new data types.")]
        private CampaignContext.NonCombatEncounterStatus previousNonCombatEncounterState { get; set; } = CampaignContext.NonCombatEncounterStatus.NotInNonCombatEncounter;

        [Obsolete("Should transition to extrapolating this information from " + nameof(previousCombatTurnTaker))]
        private CombatContext.TurnStatus previousCombatTurnState { get; set; } = CombatContext.TurnStatus.NotInCombat;
        private Entity previousCombatTurnTaker { get; set; } = null;

        [Obsolete("Should transition to " + nameof(CurrentGameState))]
        public CampaignContext CurrentCampaignContext => this.CentralGameStateControllerInstance?.CurrentCampaignContext;
        public GameState CurrentGameState => this.CentralGameStateControllerInstance?.GameplayState;
        public EncounterState CurrentEncounterState => this.CurrentGameState.CurrentEncounterState;

        [Obsolete("Transition to " + nameof(_HoveredCombatant))]
        public ICombatantTarget HoveredCombatant { get; set; } = null;
        public IChangeTarget _HoveredCombatant { get; set; } = null;

        private void Awake()
        {
            this.Annihilate();

            GlobalUpdateUX.LogTextEvent.AddListener(this.AddToLog);
        }

        private void OnEnable()
        {
        }

        private void OnDestroy()
        {
            GlobalUpdateUX.LogTextEvent.RemoveListener(AddToLog);
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(1))
            {
                this.CancelAllSelections();
            }
        }

        public PlayerUX _PlacePlayerCharacter()
        {
            if (this.PlayerUXInstance != null)
            {
                Destroy(this.PlayerUXInstance.gameObject);
                this.PlayerUXInstance = null;
            }

            this.PlayerUXInstance = Instantiate(this.PlayerRepresentationPF, this.PlayerRepresentationTransform);

            Entity campaignEntity = null;
            foreach (Entity curEntity in this.CurrentGameState.PersistentEntities)
            {
                if (curEntity.Qualities.GetNumericQuality(WellknownQualities.Faction) == WellknownFactions.Player)
                {
                    campaignEntity = curEntity;
                    break;
                }
            }

            this.PlayerUXInstance._SetFromPlayer(campaignEntity);

            this.LifeValue.text = $"{campaignEntity.Qualities.GetNumericQuality(WellknownQualities.Health, 0).ToString()} / {campaignEntity.Qualities.GetNumericQuality(WellknownQualities.MaximumHealth, 0).ToString()}";
            return this.PlayerUXInstance;
        }

        public void _CheckAndActIfGameCampaignNavigationStateChanged()
        {
            if (this.CentralGameStateControllerInstance.GameplayState == null)
            {
                this.GoNextRoomButton.SetActive(false);
                this.EndTurnButton.SetActive(false);
                this.EncounterRepresenterUXInstance.Close();
                // MouseHoverShowerPanel.CurrentContext = null;
                return;
            }

            this.CampaignChooserUXInstance.HideChooser();
            Entity currentTurnTaker = null;
            if (!(this.CurrentEncounterState != null && this.CurrentGameState.EntityTurnTakerCalculator.TryGetCurrentEntityTurn(this.CentralGameStateControllerInstance.GameplayState, out currentTurnTaker)))
            {
                currentTurnTaker = null;
            }

            if (this.CurrentGameState.PendingRewards != null && this.RewardsPanelUXInstance._Rewards != this.CurrentGameState.PendingRewards)
            {
                this._PresentAwards(this.CurrentGameState.PendingRewards);
            }

            /*

            CampaignContext.GameplayCampaignState wasPreviousCampaignState = this.previousCampaignState;
            CampaignContext.NonCombatEncounterStatus wasPreviousNonCombatState = this.previousNonCombatEncounterState;

            if (wasPreviousCampaignState == newCampaignState
                && wasPreviousNonCombatState == newNonCombatState
                && this.previousCombatTurnTaker == currentTurnTaker)
            {
                return;
            }

            this.previousCampaignState = newCampaignState;
            this.previousNonCombatEncounterState = newNonCombatState;
            this.previousCombatTurnTaker = currentTurnTaker;

            if (newCampaignState == CampaignContext.GameplayCampaignState.ClearedRoom
                || (newCampaignState == CampaignContext.GameplayCampaignState.NonCombatEncounter && newNonCombatState == CampaignContext.NonCombatEncounterStatus.AllowedToLeave))
            {
                this.GoNextRoomButton.SetActive(true);
            }
            else
            {
                this.RewardsPanelUXInstance.gameObject.SetActive(false);
                this.ShopPanelUXInstance.gameObject.SetActive(false);
                this.GoNextRoomButton.SetActive(false);
            }

            if (newCampaignState == CampaignContext.GameplayCampaignState.NonCombatEncounter)
            {
                if (wasPreviousCampaignState != CampaignContext.GameplayCampaignState.NonCombatEncounter &&
                    this.CurrentCampaignContext != null &&
                    this.CurrentCampaignContext._CurrentEncounter != null
                    )
                {
                    if (this.CurrentCampaignContext._CurrentEncounter.HasEncounterDialogue)
                    {
                        this.ShopPanelUXInstance.gameObject.SetActive(false);
                        this.EncounterRepresenterUXInstance._RepresentEncounter(this.CurrentCampaignContext._CurrentEncounter, this.CurrentCampaignContext.GameplayState);
                    }
                    else if (this.CurrentCampaignContext._CurrentEncounter.IsShopEncounter)
                    {
                        this.EncounterRepresenterUXInstance.Close();
                        this._ShowShopPanel(this.CurrentCampaignContext._CurrentEncounter.GetShop());
                    }
                }
            }
            else
            {
                this.ShopPanelUXInstance.gameObject.SetActive(false);
                this.EncounterRepresenterUXInstance.Close();
            }

            if (newCampaignState == CampaignContext.GameplayCampaignState.InCombat)
            {
                this.CombatUXFolder.SetActive(true);
                this.RewardsPanelUXInstance.gameObject.SetActive(false);
                this.ShopPanelUXInstance.gameObject.SetActive(false);

                if (wasPreviousCampaignState != CampaignContext.GameplayCampaignState.InCombat)
                {
                    this.CombatTurnCounterInstance._BeginHandlingCombat();
                }

                if (currentTurnTaker.Qualities.GetNumericQuality(WellknownQualities.Faction) == WellknownFactions.Player)
                {
                    this.EndTurnButton.SetActive(true);
                }
                else
                {
                    this.EndTurnButton.SetActive(false);
                }
            }
            else
            {
                if (wasPreviousCampaignState == CampaignContext.GameplayCampaignState.InCombat)
                {
                    this.CombatTurnCounterInstance.EndHandlingCombat();
                }

                this.EndTurnButton.SetActive(false);
                this.CombatUXFolder.SetActive(false);
            }

            if (newCampaignState == CampaignContext.GameplayCampaignState.MakingRouteChoice)
            {
                this._PresentNextRouteChoice();
            }
            else
            {
                this.ClearRouteUX();
            }

            if (newCampaignState == CampaignContext.GameplayCampaignState.Victory)
            {
                GlobalUpdateUX.LogTextEvent?.Invoke($"Victory!! There are no more nodes in this route! Reset game to continue from beginning.", GlobalUpdateUX.LogType.GameEvent);
            }
            */
        }

        public void _UpdateUX()
        {
            if (this.CardBrowserUXInstance.gameObject.activeInHierarchy)
            {
                this.AllCardsBrowserButton.SetActive(false);
            }
            else
            {
                this.AllCardsBrowserButton.SetActive(true);
            }

            this._CheckAndActIfGameCampaignNavigationStateChanged();
            this._RemoveDefeatedEntities();
            this._SetElementValueLabel();
            this._SetCurrenciesValueLabel();
            this.UpdateEnemyUX();
            this.UpdatePlayerLabelValues();
            this._RepresentTargetables();
        }

        public void _SelectTarget(IChangeTarget toSelect)
        {
            if (this.CentralGameStateControllerInstance.GameplayState.CurrentlyConsideredPlayedCard == null)
            {
                return;
            }

            throw new NotImplementedException();
            // TODO: INSTRUCT PLAY CARD WITH SUBMITTED TARGETS
            // this.CombatTurnCounterInstance.PlayCard(this.CurrentSelectedCard.RepresentedCard, toSelect);
            // this.CurrentSelectedCard = null;
        }

        public void _SelectCurrentCard(DisplayedCardUX toSelect)
        {
            if (this.CentralGameStateControllerInstance.GameplayState.CurrentEncounterState == null)
            {
                return;
            }

            if (this.PlayerIsCurrentlyAnimating)
            {
                GlobalUpdateUX.LogTextEvent.Invoke($"Player is currently animating, please wait until finished.", GlobalUpdateUX.LogType.GameEvent);
                return;
            }

            if (this.CurrentSelectedCard != null)
            {
                this.CurrentSelectedCard.DisableSelectionGlow();
                this.CurrentSelectedCard = null;
            }

            this.CurrentSelectedCard = toSelect;
            this.CurrentSelectedCard.EnableSelectionGlow();
            this._AppointTargetableIndicatorsToValidTargets(toSelect._RepresentedCard);
            GlobalUpdateUX.LogTextEvent.Invoke($"Selected card {toSelect._RepresentedCard.Name}", GlobalUpdateUX.LogType.GameEvent);
        }

        public void AddToLog(string text, GlobalUpdateUX.LogType logType)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            if (!text.EndsWith("\n"))
            {
                text += "\n";
            }
            this.Log.text += text;

            const int maximumLogSize = 10000;
            if (this.Log.text.Length > maximumLogSize)
            {
                this.Log.text = this.Log.text.Substring(this.Log.text.Length - maximumLogSize, maximumLogSize);
            }

            Debug.Log(text);
        }

        public void CancelAllSelections()
        {
            this.CurrentSelectedCard?.DisableSelectionGlow();
            this.CurrentSelectedCard = null;

            this.PlayerHandRepresenter._DeselectSelectedCard();

            if (this.CardBrowserUXInstance.RemainingCardsToChoose > 0)
            {
                this.CardBrowserUXInstance.Close();
            }

            ClearAllTargetableIndicators();
        }

        public void _ShowRewardsPanel(List<PickReward> toReward)
        {
            this.RewardsPanelUXInstance.gameObject.SetActive(true);
            this.RewardsPanelUXInstance._SetReward(toReward);
            this._UpdateUX();
        }

        public void _ShowShopPanel(IReadOnlyList<IShopEntry> itemsInShop)
        {
            this.ShopPanelUXInstance.gameObject.SetActive(true);
            this.ShopPanelUXInstance._SetShopItems(itemsInShop);
        }

        private IEnumerator _AnimateEnemyTurnsInternal(Action continuationAction)
        {
            throw new NotImplementedException();
        }

        public IEnumerator _AnimateCardPlay(CardInstance toPlay, IChangeTarget target)
        {
            yield return _AnimateCardPlayInternal(toPlay, target);
        }

        private IEnumerator _AnimateCardPlayInternal(CardInstance toPlay, IChangeTarget target)
        {
            PlayerIsCurrentlyAnimating = true;

            // TODO: REINTRODUCE CARD ANIMATIONS
            yield return _AnimateAction(this.PlayerUXInstance, target);

            PlayerIsCurrentlyAnimating = false;
        }

        private IEnumerator _AnimateAction(IAnimationPuppet puppet, IChangeTarget target)
        {
            IAnimationPuppet targetPuppet = null;

            if (target is Entity entity)
            {
                if (entity.Qualities.GetNumericQuality(WellknownQualities.Faction) == WellknownFactions.Player)
                {
                    targetPuppet = this.PlayerUXInstance;
                }
                else
                {
                    targetPuppet = this.EnemyRepresenterUX._SpawnedEnemiesLookup[entity];
                }
            }

            if (targetPuppet == null || targetPuppet == puppet)
            {
                yield return this.AnimationRunnerController.AnimateUpwardNod(
                    puppet
                );
            }
            else
            {
                yield return this.AnimationRunnerController.AnimateShoveAttack(
                    puppet,
                    targetPuppet
                );
            }
        }

        void UpdatePlayerLabelValues()
        {
            if (this.CurrentCampaignContext?.CampaignPlayer == null)
            {
                this.LifeValue.text = "0";
                this.PlayerStatusEffectUXHolderInstance.Annihilate();

                return;
            }

            this.LifeValue.text = this.CurrentCampaignContext.CampaignPlayer.CurrentHealth.ToString();
            this.PlayerStatusEffectUXHolderInstance._SetStatusEffects(
                this.CentralGameStateControllerInstance?.CurrentCampaignContext?._CampaignPlayer.AppliedStatusEffects.Values,
                this._StatusEffectClicked);
        }

        [Obsolete("Transition to " + nameof(_SetElementValueLabel))]
        private void SetElementValueLabel()
        {
            if (this.CentralGameStateControllerInstance.CurrentCampaignContext == null ||
                this.CentralGameStateControllerInstance.CurrentCampaignContext.CurrentCombatContext == null ||
                this.CentralGameStateControllerInstance.CurrentCampaignContext.CurrentCombatContext.ElementResourceCounts == null || this.CentralGameStateControllerInstance.CurrentCampaignContext.CurrentCombatContext.ElementResourceCounts.Count == 0)
            {
                this.ElementsValue.text = "None";
                return;
            }

            string startingSeparator = "";
            StringBuilder compositeElements = new StringBuilder();
            foreach (SFDDCards.Element element in this.CurrentCampaignContext.CurrentCombatContext.ElementResourceCounts.Keys)
            {
                compositeElements.Append($"{startingSeparator}{this.CurrentCampaignContext.CurrentCombatContext.ElementResourceCounts[element]}\u00A0{element.GetNameOrIcon()}");
                startingSeparator = ", ";
            }

            this.ElementsValue.text = compositeElements.ToString();
        }

        private void _SetElementValueLabel()
        {
            if (this.CentralGameStateControllerInstance.GameplayState == null)
            {
                this.ElementsValue.text = "None";
                return;
            }

            string startingSeparator = "";
            StringBuilder compositeElements = new StringBuilder();
            foreach (SpaceDeck.GameState.Minimum.Element element in this.CentralGameStateControllerInstance.GameplayState.Elements.Keys)
            {
                compositeElements.Append($"{startingSeparator}{this.CentralGameStateControllerInstance.GameplayState.Elements[element]}\u00A0{SpriteLookup.GetNameAndMaybeIcon(element)}");
                startingSeparator = ", ";
            }

            this.ElementsValue.text = compositeElements.ToString();
        }

        [Obsolete("Transition to " + nameof(_SetCurrenciesValueLabel))]
        private void SetCurrenciesValueLabel()
        {
            if (this.CentralGameStateControllerInstance.CurrentCampaignContext == null || this.CentralGameStateControllerInstance.CurrentCampaignContext.CurrencyCounts.Count == 0)
            {
                this.CurrenciesValue.text = "None";
                return;
            }

            string startingSeparator = "";
            StringBuilder compositeCurrencies = new StringBuilder();
            foreach (CurrencyImport currency in this.CentralGameStateControllerInstance.CurrentCampaignContext.CurrencyCounts.Keys)
            {
                compositeCurrencies.Append($"{startingSeparator}{this.CentralGameStateControllerInstance.CurrentCampaignContext.CurrencyCounts[currency]}\u00A0{currency.GetNameAndMaybeIcon()}");
                startingSeparator = ", ";
            }

            this.CurrenciesValue.text = compositeCurrencies.ToString();
        }

        private void _SetCurrenciesValueLabel()
        {
            if (this.CentralGameStateControllerInstance.GameplayState == null || this.CentralGameStateControllerInstance.GameplayState.Currencies.Count == 0)
            {
                this.CurrenciesValue.text = "None";
                return;
            }

            string startingSeparator = "";
            StringBuilder compositeCurrencies = new StringBuilder();
            foreach (Currency currency in this.CentralGameStateControllerInstance.GameplayState.Currencies.Keys)
            {
                compositeCurrencies.Append($"{startingSeparator}{this.CentralGameStateControllerInstance.GameplayState.Currencies[currency]}\u00A0{SpriteLookup.GetNameAndMaybeIcon(currency)}");
                startingSeparator = ", ";
            }

            this.CurrenciesValue.text = compositeCurrencies.ToString();
        }

        private void ClearAllTargetableIndicators()
        {
            if (this.ActiveIndicators != null)
            {
                for (int ii = 0; ii < this.ActiveIndicators.Count; ii++)
                {
                    Destroy(this.ActiveIndicators[ii].gameObject);
                }

                this.ActiveIndicators.Clear();
            }

            this.NoTargetsIndicator.gameObject.SetActive(false);
            this.AllFoeTargetsIndicator.gameObject.SetActive(false);
        }

        [Obsolete("Transition to " + nameof(_AppointTargetableIndicatorsToValidTargets))]
        private void AppointTargetableIndicatorsToValidTargets(Card toTarget)
        {
            throw new System.Exception("OBSOLETE");
        }

        private void _AppointTargetableIndicatorsToValidTargets(CardInstance toTarget)
        {
            this.ClearAllTargetableIndicators();

            List<IChangeTarget> remainingTargets = new List<IChangeTarget>(toTarget.GetPossibleTargets(this.CentralGameStateControllerInstance.CurrentCampaignContext.GameplayState));

            if (remainingTargets.Count > 0)
            {
                // TODO: is this the all foes target?
                foreach (IChangeTarget target in remainingTargets)
                {
                    if (target == NobodyTarget.Instance)
                    {
                        this.NoTargetsIndicator._SetFromTarget(target, _SelectTarget, _BeginHoverTarget, _EndHoverTarget);
                        this.NoTargetsIndicator.gameObject.SetActive(true);
                    }
                    else
                    {
                        IReadOnlyList<Entity> representedEntities = new List<Entity>(target.GetRepresentedEntities(this.CentralGameStateControllerInstance.CurrentCampaignContext.GameplayState));

                        foreach (Entity representedEntity in representedEntities)
                        {
                            if (representedEntity == this.CentralGameStateControllerInstance.CurrentCampaignContext._CampaignPlayer)
                            {
                                PlayerUX playerUx = this.PlayerUXInstance;
                                TargetableIndicator playerIndicator = Instantiate(this.SingleCombatantTargetableIndicatorPF, playerUx.transform);
                                playerIndicator._SetFromTarget(representedEntities[0], this._SelectTarget, _BeginHoverTarget, _EndHoverTarget);
                                this.ActiveIndicators.Add(playerIndicator);
                            }
                            else
                            {
                                if (this.EnemyRepresenterUX._SpawnedEnemiesLookup.TryGetValue(representedEntity, out EnemyUX enemyUX))
                                {
                                    TargetableIndicator playerIndicator = Instantiate(this.SingleCombatantTargetableIndicatorPF, enemyUX.transform);
                                    playerIndicator._SetFromTarget(representedEntities[0], this._SelectTarget, _BeginHoverTarget, _EndHoverTarget);
                                    this.ActiveIndicators.Add(playerIndicator);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
            }
        }

        void UpdateEnemyUX()
        {
            if (this.CentralGameStateControllerInstance?.CurrentCampaignContext?._CurrentEncounter?.EncounterEntities == null)
            {
                return;
            }

            foreach (Entity curEnemy in this.CentralGameStateControllerInstance?.CurrentCampaignContext._CurrentEncounter.EncounterEntities)
            {
                if (!this.EnemyRepresenterUX._SpawnedEnemiesLookup.TryGetValue(curEnemy, out EnemyUX value))
                {
                    // It could be that the UpdateUX call was made before these enemies are spawned in to the game
                    // In that case, just continue
                    continue;
                }

                value._UpdateUX(this.CentralGameStateControllerInstance);
            }
        }

        public void Annihilate()
        {
            this.PlayerHandRepresenter.Annihilate();
            this.EnemyRepresenterUX.Annihilate();
            this.CardBrowserUXInstance.Close();

            for (int ii = this.PlayerRepresentationTransform.childCount - 1; ii >= 0; ii--)
            {
                Destroy(this.PlayerRepresentationTransform.GetChild(ii).gameObject);
            }

            if (this.PlayerUXInstance != null)
            {
                Destroy(this.PlayerUXInstance.gameObject);
            }

            this.ShopPanelUXInstance.gameObject.SetActive(false);
            this.RewardsPanelUXInstance.gameObject.SetActive(false);
            this.PlayerStatusEffectUXHolderInstance.Annihilate();
        }

        void _RepresentTargetables()
        {
            if (this.CurrentGameState?.CurrentEncounterState == null)
            {
                ClearAllTargetableIndicators();
                return;
            }

            if (this.CurrentSelectedCard == null)
            {
                ClearAllTargetableIndicators();
                return;
            }

            this._AppointTargetableIndicatorsToValidTargets(this.CurrentSelectedCard._RepresentedCard);
        }

        public void EndTurn()
        {
            this.CancelAllSelections();

            if (this.CentralGameStateControllerInstance.GameplayState.EntityTurnTakerCalculator.TryGetCurrentEntityTurn(this.CurrentGameState, out Entity currentTurnEntity) && currentTurnEntity != this.PlayerUXInstance._RepresentedPlayer)
            {
                GlobalUpdateUX.LogTextEvent.Invoke($"It's not the player's turn, can't end turn.", GlobalUpdateUX.LogType.GameEvent);
                return;
            }

            this.CombatTurnCounterInstance._EndPlayerTurn();
        }

        public void _PresentAwards(List<PickReward> toPresent)
        {
            this.CurrentGameState.PendingRewards = null;
            this.CancelAllSelections();
            this._ShowRewardsPanel(toPresent);
        }

        void _PresentNextRouteChoice()
        {
            this.CancelAllSelections();

            SpaceDeck.GameState.Minimum.ChoiceNode campaignNode = this.CurrentGameState.GetCampaignCurrentNode();

            if (campaignNode == null)
            {
                GlobalUpdateUX.LogTextEvent?.Invoke($"The next campaign node is null. Cannot continue with UX.", GlobalUpdateUX.LogType.RuntimeError);
                return;
            }

            this.ChoiceUXFolder.SetActive(true);
            this.ChoiceSelectorUX._RepresentNode(campaignNode);
        }

        void ClearRouteUX()
        {
            this.CancelAllSelections();
            this.ChoiceUXFolder.SetActive(false);
        }

        public void _NodeIsChosen(SpaceDeck.GameState.Minimum.ChoiceNodeOption option)
        {
            this.CancelAllSelections();
            this.CentralGameStateControllerInstance.GameplayState.MakeChoiceNodeDecision(option);
        }

        public void _ProceedToNextRoom()
        {
            this.CancelAllSelections();
            if (this.CentralGameStateControllerInstance.GameplayState.StartNextRoomFromCampaign(out SpaceDeck.GameState.Minimum.ChoiceNode nextChoice))
            {
                this._PresentNextRouteChoice();
            }
        }

        public void _RouteChosen(Route chosenRoute)
        {
            this.CancelAllSelections();
            this.CentralGameStateControllerInstance._RouteChosen(chosenRoute);
        }

        public void _ShowCampaignChooser()
        {
            this.CampaignChooserUXInstance._ShowChooser();
        }

        private void _RemoveDefeatedEntities()
        {
            if (this.CentralGameStateControllerInstance?.GameplayState.CurrentEncounterState == null)
            {
                foreach (Entity key in new List<Entity>(this.EnemyRepresenterUX._SpawnedEnemiesLookup.Keys))
                {
                    this.EnemyRepresenterUX._RemoveEnemy(key);
                }

                return;
            }

            foreach (Entity curEnemy in new List<Entity>(this.EnemyRepresenterUX._SpawnedEnemiesLookup.Keys))
            {
                if (!this.CurrentEncounterState.EncounterEntities.Contains(curEnemy))
                {
                    this.EnemyRepresenterUX._RemoveEnemy(curEnemy);
                }
            }
        }

        public void _StatusEffectClicked(SpaceDeck.GameState.Minimum.AppliedStatusEffect representingEffect)
        {
            if (representingEffect.TriggerOnEventIds.Contains(WellknownGameStateEvents.Activated))
            {
                GameStateEventTrigger trigger = new GameStateEventTrigger(WellknownGameStateEvents.Activated);
                if (representingEffect.TryApplyStatusEffect(trigger, this.CentralGameStateControllerInstance.GameplayState, out List<GameStateChange> changes))
                {
                    SpaceDeck.GameState.Execution.GameStateDelta delta = new SpaceDeck.GameState.Execution.GameStateDelta(this.CentralGameStateControllerInstance.GameplayState, changes);
                    GameStateDeltaApplier.ApplyGameStateDelta(this.CentralGameStateControllerInstance.GameplayState, delta);
                }
            }
        }

        public void _OpenAllCardsBrowser()
        {
            if (this.CardBrowserUXInstance.gameObject.activeInHierarchy)
            {
                GlobalUpdateUX.LogTextEvent.Invoke($"The card browser is already open. Perhaps you need to respond to an event.", GlobalUpdateUX.LogType.UserError);
                return;
            }

            this.CardBrowserUXInstance.gameObject.SetActive(true);
            this.CardBrowserUXInstance.SetLabelText("Now Viewing: All Cards");
            this.CardBrowserUXInstance._SetFromCards(SpaceDeck.Models.Databases.CardDatabase.GetOneOfEveryCard());
        }

        public void _OpenDiscardCardsBrowser()
        {
            if (this.CardBrowserUXInstance.gameObject.activeInHierarchy)
            {
                GlobalUpdateUX.LogTextEvent.Invoke($"The card browser is already open. Perhaps you need to respond to an event.", GlobalUpdateUX.LogType.UserError);
                return;
            }

            if (this.CentralGameStateControllerInstance?.GameplayState?.CurrentEncounterState == null)
            {
                return;
            }

            this.CardBrowserUXInstance.gameObject.SetActive(true);
            this.CardBrowserUXInstance.SetLabelText("Now Viewing: Cards in Discard");
            this.CardBrowserUXInstance._SetFromCards(this.CentralGameStateControllerInstance.GameplayState.CurrentEncounterState.GetZoneCards(WellknownZones.Discard));
        }

        public void _OpenDeckCardsBrowser()
        {
            if (this.CardBrowserUXInstance.gameObject.activeInHierarchy)
            {
                GlobalUpdateUX.LogTextEvent.Invoke($"The card browser is already open. Perhaps you need to respond to an event.", GlobalUpdateUX.LogType.UserError);
                return;
            }

            if (this.CentralGameStateControllerInstance?.GameplayState == null)
            {
                return;
            }

            List<CardInstance> cardsInDeck = new List<CardInstance>(this.CentralGameStateControllerInstance.GameplayState.CardsInDeck);

            if (this.CentralGameStateControllerInstance?.GameplayState.CurrentEncounterState != null)
            {
                cardsInDeck = new List<CardInstance>(this.CurrentEncounterState.GetZoneCards(WellknownZones.Deck));
            }

            cardsInDeck.Sort((CardInstance a, CardInstance b) => a.Name.CompareTo(b.Name));

            this.CardBrowserUXInstance.gameObject.SetActive(true);
            this.CardBrowserUXInstance.SetLabelText("Now Viewing: Cards in Deck");
            this.CardBrowserUXInstance._SetFromCards(cardsInDeck);
        }

        public void _OpenExileCardsBrowser()
        {
            if (this.CardBrowserUXInstance.gameObject.activeInHierarchy)
            {
                GlobalUpdateUX.LogTextEvent.Invoke($"The card browser is already open. Perhaps you need to respond to an event.", GlobalUpdateUX.LogType.UserError);
                return;
            }

            if (this.CentralGameStateControllerInstance?.GameplayState.CurrentEncounterState == null)
            {
                return;
            }

            List<CardInstance> cardsInExile = new List<CardInstance>(this.CentralGameStateControllerInstance.GameplayState.GetCardsInZone(WellknownZones.Exile));
            cardsInExile.Sort((CardInstance a, CardInstance b) => a.Name.CompareTo(b.Name));

            this.CardBrowserUXInstance.gameObject.SetActive(true);
            this.CardBrowserUXInstance.SetLabelText("Now Viewing: Cards in Exile");
            this.CardBrowserUXInstance._SetFromCards(cardsInExile);
        }

        public void _BeginHoverTarget(IChangeTarget target)
        {
            this._HoveredCombatant = target;
        }

        public void _EndHoverTarget(IChangeTarget target)
        {
            if (this._HoveredCombatant == target)
            {
                this._HoveredCombatant = null;
            }
        }

        public void EncounterDialogueComplete(EncounterState completed)
        {
            if (completed == this.CentralGameStateControllerInstance.GameplayState.CurrentEncounterState)
            {
                this.EncounterRepresenterUXInstance.Close();
                this._ProceedToNextRoom();
            }
        }
    }
}