namespace SpaceDeck.UX
{
    using SFDDCards;
    using SFDDCards.Evaluation.Actual;
    using SFDDCards.ImportModels;
    using SFDDCards.UX;
    using SpaceDeck.GameState.Execution;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Utility.Wellknown;
    using SpaceDeck.UX;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;
    using System.Text.RegularExpressions;
    using UnityEngine;

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

        private CampaignContext.GameplayCampaignState previousCampaignState { get; set; } = CampaignContext.GameplayCampaignState.NotStarted;
        private CampaignContext.NonCombatEncounterStatus previousNonCombatEncounterState { get; set; } = CampaignContext.NonCombatEncounterStatus.NotInNonCombatEncounter;

        [Obsolete("Should transition to extrapolating this information from " + nameof(previousCombatTurnTaker))]
        private CombatContext.TurnStatus previousCombatTurnState { get; set; } = CombatContext.TurnStatus.NotInCombat;
        private Entity previousCombatTurnTaker { get; set; } = null;

        public CampaignContext CurrentCampaignContext => this.CentralGameStateControllerInstance?.CurrentCampaignContext;

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
            GlobalUpdateUX.UpdateUXEvent.AddListener(UpdateUX);
        }

        private void OnDestroy()
        {
            GlobalUpdateUX.UpdateUXEvent.RemoveListener(UpdateUX);
            GlobalUpdateUX.LogTextEvent.RemoveListener(AddToLog);
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(1))
            {
                this.CancelAllSelections();
            }
        }

        [Obsolete("Should transition to " + nameof(_PlacePlayerCharacter))]
        public void PlacePlayerCharacter()
        {
            if (this.PlayerUXInstance != null)
            {
                Destroy(this.PlayerUXInstance.gameObject);
                this.PlayerUXInstance = null;
            }

            this.PlayerUXInstance = Instantiate(this.PlayerRepresentationPF, this.PlayerRepresentationTransform);
            this.PlayerUXInstance.SetFromPlayer(this.CurrentCampaignContext.CampaignPlayer);

            this.LifeValue.text = $"{this.CentralGameStateControllerInstance.CurrentCampaignContext.CampaignPlayer.CurrentHealth} / {this.CentralGameStateControllerInstance.CurrentCampaignContext.CampaignPlayer.MaxHealth}";
        }

        public void _PlacePlayerCharacter()
        {
            if (this.PlayerUXInstance != null)
            {
                Destroy(this.PlayerUXInstance.gameObject);
                this.PlayerUXInstance = null;
            }

            this.PlayerUXInstance = Instantiate(this.PlayerRepresentationPF, this.PlayerRepresentationTransform);

            Entity campaignEntity = this.CurrentCampaignContext._CampaignPlayer;
            this.PlayerUXInstance._SetFromPlayer(campaignEntity);

            this.LifeValue.text = $"{campaignEntity.Qualities.GetNumericQuality(WellknownQualities.Health, 0).ToString()} / {campaignEntity.Qualities.GetNumericQuality(WellknownQualities.MaximumHealth, 0).ToString()}";
        }

        [Obsolete("Transition to " + nameof(_CheckAndActIfGameCampaignNavigationStateChanged))]
        public void CheckAndActIfGameCampaignNavigationStateChanged()
        {
            if (this.CentralGameStateControllerInstance.CurrentCampaignContext == null)
            {
                this.GoNextRoomButton.SetActive(false);
                this.EndTurnButton.SetActive(false);
                this.EncounterRepresenterUXInstance.Close();
                // MouseHoverShowerPanel.CurrentContext = null;
                return;
            }

            this.CampaignChooserUXInstance.HideChooser();

            CampaignContext.GameplayCampaignState newCampaignState = this.CurrentCampaignContext.CurrentGameplayCampaignState;
            CampaignContext.NonCombatEncounterStatus newNonCombatState = this.CurrentCampaignContext.CurrentNonCombatEncounterStatus;
            CombatContext.TurnStatus newTurnState = this.CurrentCampaignContext.CurrentCombatContext == null ? CombatContext.TurnStatus.NotInCombat : this.CurrentCampaignContext.CurrentCombatContext.CurrentTurnStatus;

            // TODO: DISMANTLE
            // if (this.CurrentCampaignContext?.PendingRewards != null && this.RewardsPanelUXInstance.Rewards != this.CurrentCampaignContext?.PendingRewards)
            // {
            //     this.PresentAwards(this.CurrentCampaignContext.PendingRewards);
            // }

            CampaignContext.GameplayCampaignState wasPreviousCampaignState = this.previousCampaignState;
            CampaignContext.NonCombatEncounterStatus wasPreviousNonCombatState = this.previousNonCombatEncounterState;
            CombatContext.TurnStatus wasPreviousTurnState = this.previousCombatTurnState;

            if (wasPreviousCampaignState == newCampaignState
                && wasPreviousNonCombatState == newNonCombatState
                && wasPreviousTurnState == newTurnState)
            {
                return;
            }

            this.previousCampaignState = newCampaignState;
            this.previousNonCombatEncounterState = newNonCombatState;
            this.previousCombatTurnState = newTurnState;

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
                    this.CurrentCampaignContext.CurrentEncounter != null
                    )
                {
                    if (this.CurrentCampaignContext.CurrentEncounter.BasedOn.EncounterScripts != null && this.CurrentCampaignContext.CurrentEncounter.BasedOn.EncounterScripts.Count > 0)
                    {
                        this.ShopPanelUXInstance.gameObject.SetActive(false);
                        this.EncounterRepresenterUXInstance.RepresentEncounter(this.CurrentCampaignContext.CurrentEncounter);
                    }
                    else if (this.CurrentCampaignContext.CurrentEncounter.BasedOn.IsShopEncounter)
                    {
                        this.EncounterRepresenterUXInstance.Close();
                        // TODO: DISMANTLE
                        // this.ShowShopPanel(this.CurrentCampaignContext.CurrentEncounter.GetShop(this.CurrentCampaignContext).ToArray());
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
                    this.CombatTurnCounterInstance.BeginHandlingCombat();
                }

                if (newTurnState == CombatContext.TurnStatus.PlayerTurn)
                {
                    this.EndTurnButton.SetActive(true);
                }
                else
                {
                    this.EndTurnButton.SetActive(false);
                }

                // MouseHoverShowerPanel.CurrentContext = new ReactionWindowContext(this.CurrentCampaignContext, KnownReactionWindows.ConsideringPlayingFromHand, this.CurrentCampaignContext.CurrentCombatContext.CombatPlayer, combatantTarget: null, playedFromZone: "hand");
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
                this.PresentNextRouteChoice();
            }
            else
            {
                this.ClearRouteUX();
            }

            if (newCampaignState == CampaignContext.GameplayCampaignState.Victory)
            {
                GlobalUpdateUX.LogTextEvent?.Invoke($"Victory!! There are no more nodes in this route! Reset game to continue from beginning.", GlobalUpdateUX.LogType.GameEvent);
            }
        }


        public void _CheckAndActIfGameCampaignNavigationStateChanged()
        {
            if (this.CentralGameStateControllerInstance.CurrentCampaignContext == null)
            {
                this.GoNextRoomButton.SetActive(false);
                this.EndTurnButton.SetActive(false);
                this.EncounterRepresenterUXInstance.Close();
                // MouseHoverShowerPanel.CurrentContext = null;
                return;
            }

            this.CampaignChooserUXInstance.HideChooser();

            CampaignContext.GameplayCampaignState newCampaignState = this.CurrentCampaignContext.CurrentGameplayCampaignState;
            CampaignContext.NonCombatEncounterStatus newNonCombatState = this.CurrentCampaignContext.CurrentNonCombatEncounterStatus;
            Entity currentTurnTaker = null;
            if (!(this.CurrentCampaignContext._CurrentEncounter != null && this.CurrentCampaignContext.GameplayState.EntityTurnTakerCalculator.TryGetCurrentEntityTurn(this.CurrentCampaignContext.GameplayState, out currentTurnTaker)))
            {
                currentTurnTaker = null;
            }

            if (this.CurrentCampaignContext?._PendingRewards != null && this.RewardsPanelUXInstance._Rewards != this.CurrentCampaignContext?._PendingRewards)
            {
                this._PresentAwards(this.CurrentCampaignContext._PendingRewards);
            }

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
                        // TODO: Shop Panel for new codebase
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
                this.PresentNextRouteChoice();
            }
            else
            {
                this.ClearRouteUX();
            }

            if (newCampaignState == CampaignContext.GameplayCampaignState.Victory)
            {
                GlobalUpdateUX.LogTextEvent?.Invoke($"Victory!! There are no more nodes in this route! Reset game to continue from beginning.", GlobalUpdateUX.LogType.GameEvent);
            }
        }

        [Obsolete("Transition to " + nameof(_UpdateUX))]
        public void UpdateUX(CampaignContext forCampaign)
        {
            if (this.CardBrowserUXInstance.gameObject.activeInHierarchy)
            {
                this.AllCardsBrowserButton.SetActive(false);
            }
            else
            {
                this.AllCardsBrowserButton.SetActive(true);
            }

            this.CheckAndActIfGameCampaignNavigationStateChanged();
            this.RemoveDefeatedEntities();
            this.SetElementValueLabel();
            this.SetCurrenciesValueLabel();
            this.UpdateEnemyUX();
            this.UpdatePlayerLabelValues();
            this.RepresentTargetables();
        }

        public void _UpdateUX(CampaignContext forCampaign)
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
            this.SetElementValueLabel();
            this.SetCurrenciesValueLabel();
            this.UpdateEnemyUX();
            this.UpdatePlayerLabelValues();
            this.RepresentTargetables();
        }

        public void _SelectTarget(IChangeTarget toSelect)
        {
            if (this.CentralGameStateControllerInstance.CurrentCampaignContext.GameplayState.CurrentlyConsideredPlayedCard == null)
            {
                return;
            }

            throw new NotImplementedException();
            // TODO: INSTRUCT PLAY CARD WITH SUBMITTED TARGETS
            // this.CombatTurnCounterInstance.PlayCard(this.CurrentSelectedCard.RepresentedCard, toSelect);
            // this.CurrentSelectedCard = null;
        }

        public void SelectCurrentCard(DisplayedCardUX toSelect)
        {
            if (this.CurrentCampaignContext.CurrentCombatContext == null ||
                this.CurrentCampaignContext.CurrentCombatContext.CurrentTurnStatus != CombatContext.TurnStatus.PlayerTurn)
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
            this.AppointTargetableIndicatorsToValidTargets(toSelect.RepresentedCard);
            GlobalUpdateUX.LogTextEvent.Invoke($"Selected card {toSelect.RepresentedCard.Name}", GlobalUpdateUX.LogType.GameEvent);
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

            this.PlayerHandRepresenter.DeselectSelectedCard();

            if (this.CardBrowserUXInstance.RemainingCardsToChoose > 0)
            {
                this.CardBrowserUXInstance.Close();
            }

            ClearAllTargetableIndicators();
        }

        public void AnimateEnemyTurns(Action continuationAction)
        {
            this.StartCoroutine(AnimateEnemyTurnsInternal(continuationAction));
        }

        public void _ShowRewardsPanel(SpaceDeck.GameState.Minimum.Reward cardsToReward)
        {
            this.RewardsPanelUXInstance.gameObject.SetActive(true);
            this.RewardsPanelUXInstance._SetReward(cardsToReward);
            this._UpdateUX(this.CurrentCampaignContext);
        }

        public void _ShowShopPanel(IReadOnlyList<IShopEntry> itemsInShop)
        {
            this.ShopPanelUXInstance.gameObject.SetActive(true);
            this.ShopPanelUXInstance._SetShopItems(itemsInShop);
        }

        [Obsolete("Should transition to " + nameof(_AnimateEnemyTurnsInternal))]
        private IEnumerator AnimateEnemyTurnsInternal(Action continuationAction)
        {
            foreach (Enemy curEnemy in this.CurrentCampaignContext.CurrentCombatContext.Enemies)
            {
                yield return AnimateAction(this.EnemyRepresenterUX.SpawnedEnemiesLookup[curEnemy], curEnemy.Intent, curEnemy.Intent.PrecalculatedTarget);
            }

            continuationAction.Invoke();
        }

        private IEnumerator _AnimateEnemyTurnsInternal(Action continuationAction)
        {
            throw new NotImplementedException();
        }

        public IEnumerator AnimateCardPlay(Card toPlay, ICombatantTarget target)
        {
            yield return AnimateCardPlayInternal(toPlay, target);
        }

        private IEnumerator AnimateCardPlayInternal(Card toPlay, ICombatantTarget target)
        {
            PlayerIsCurrentlyAnimating = true;

            yield return AnimateAction(this.PlayerUXInstance, toPlay, target);

            PlayerIsCurrentlyAnimating = false;
        }

        private IEnumerator AnimateAction(IAnimationPuppet puppet, IAttackTokenHolder attack, ICombatantTarget target)
        {
            IAnimationPuppet targetPuppet = null;

            if (target is Player)
            {
                targetPuppet = this.PlayerUXInstance;
            }
            else if (target is Enemy targetEnemy)
            {
                targetPuppet = this.EnemyRepresenterUX.SpawnedEnemiesLookup[targetEnemy];
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
            this.ClearAllTargetableIndicators();

            List<ICombatantTarget> possibleTargets = new List<ICombatantTarget>();
            possibleTargets.Add(this.CentralGameStateControllerInstance?.CurrentCampaignContext?.CampaignPlayer);

            foreach (Enemy curEnemy in this.EnemyRepresenterUX.SpawnedEnemiesLookup.Keys)
            {
                possibleTargets.Add(curEnemy);
            }

            AllFoesTarget allFoesTarget = new AllFoesTarget(new List<ICombatantTarget>(this.EnemyRepresenterUX.SpawnedEnemiesLookup.Keys));
            possibleTargets.Add(allFoesTarget);

            List<ICombatantTarget> remainingTargets = ScriptTokenEvaluator.GetTargetsThatCanBeTargeted(this.CentralGameStateControllerInstance?.CurrentCampaignContext?.CampaignPlayer, toTarget, possibleTargets);

            if (remainingTargets.Count > 0)
            {
                if (remainingTargets[0] is AllFoesTarget)
                {
                    // TODO: DISMANTLE
                    // this.AllFoeTargetsIndicator.SetFromTarget(allFoesTarget, _SelectTarget, BeginHoverTarget, EndHoverTarget);
                    this.AllFoeTargetsIndicator.gameObject.SetActive(true);
                }
                else
                {
                    foreach (ICombatantTarget target in remainingTargets)
                    {
                        TargetableIndicator indicator = Instantiate(this.SingleCombatantTargetableIndicatorPF, target.UXPositionalTransform);
                        // TODO: DISMANTLE
                        // indicator.SetFromTarget(target, this._SelectTarget, BeginHoverTarget, EndHoverTarget);
                        this.ActiveIndicators.Add(indicator);
                    }
                }
            }
            else
            {
                // TODO: DISMANTLE
                // this.NoTargetsIndicator.SetFromTarget(new NoTarget(), _SelectTarget, BeginHoverTarget, EndHoverTarget);
                this.NoTargetsIndicator.gameObject.SetActive(true);
            }
        }

        private void _AppointTargetableIndicatorsToValidTargets(CardInstance toTarget)
        {
            throw new System.NotImplementedException();
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

        void RepresentTargetables()
        {
            if (this.CurrentCampaignContext?.CurrentCombatContext == null)
            {
                ClearAllTargetableIndicators();
                return;
            }

            if (this.CurrentCampaignContext.CurrentGameplayCampaignState != CampaignContext.GameplayCampaignState.InCombat)
            {
                ClearAllTargetableIndicators();
                return;
            }

            if (this.CurrentCampaignContext.CurrentCombatContext.CurrentTurnStatus != CombatContext.TurnStatus.PlayerTurn)
            {
                ClearAllTargetableIndicators();
                return;
            }

            if (this.CurrentSelectedCard == null)
            {
                ClearAllTargetableIndicators();
                return;
            }

            this.AppointTargetableIndicatorsToValidTargets(this.CurrentSelectedCard.RepresentedCard);
        }

        public void EndTurn()
        {
            this.CancelAllSelections();

            if (GlobalSequenceEventHolder.StackedSequenceEvents.Count > 0)
            {
                GlobalUpdateUX.LogTextEvent.Invoke($"Animations and events are happening, can't end turn yet.", GlobalUpdateUX.LogType.GameEvent);
                return;
            }

            if (this.CentralGameStateControllerInstance.CurrentCampaignContext.CurrentCombatContext.CurrentTurnStatus != CombatContext.TurnStatus.PlayerTurn)
            {
                GlobalUpdateUX.LogTextEvent.Invoke($"It's not the player's turn, can't end turn.", GlobalUpdateUX.LogType.GameEvent);
                return;
            }

            this.CombatTurnCounterInstance._EndPlayerTurn();
        }

        public void _PresentAwards(SpaceDeck.GameState.Minimum.Reward toPresent)
        {
            this.CurrentCampaignContext._PendingRewards = null;
            this.CancelAllSelections();
            this._ShowRewardsPanel(toPresent);
        }

        void PresentNextRouteChoice()
        {
            this.CancelAllSelections();

            SFDDCards.ChoiceNode campaignNode = this.CurrentCampaignContext.GetCampaignCurrentNode();

            if (campaignNode == null)
            {
                GlobalUpdateUX.LogTextEvent?.Invoke($"The next campaign node is null. Cannot continue with UX.", GlobalUpdateUX.LogType.RuntimeError);
                return;
            }

            this.ChoiceUXFolder.SetActive(true);
            this.ChoiceSelectorUX.RepresentNode(campaignNode);
        }

        void ClearRouteUX()
        {
            this.CancelAllSelections();
            this.ChoiceUXFolder.SetActive(false);
        }

        [Obsolete("Transition to " + nameof(_NodeIsChosen))]
        public void NodeIsChosen(SFDDCards.ChoiceNodeOption option)
        {
            this.CancelAllSelections();
            this.CentralGameStateControllerInstance.CurrentCampaignContext.MakeChoiceNodeDecision(option);
        }

        public void _NodeIsChosen(SpaceDeck.GameState.Minimum.ChoiceNodeOption option)
        {
            this.CancelAllSelections();
            this.CentralGameStateControllerInstance.CurrentCampaignContext._MakeChoiceNodeDecision(option);
        }

        public void ProceedToNextRoom()
        {
            this.CancelAllSelections();
            this.CentralGameStateControllerInstance.CurrentCampaignContext.SetCampaignState(CampaignContext.GameplayCampaignState.MakingRouteChoice);
        }

        [Obsolete("Transition to " + nameof(_RouteChosen))]
        public void RouteChosen(RouteImport chosenRoute)
        {
            this.CancelAllSelections();
            this.CentralGameStateControllerInstance.RouteChosen(chosenRoute);
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

        [Obsolete("Transition to " + nameof(_RemoveDefeatedEntities))]
        private void RemoveDefeatedEntities()
        {
            if (this.CentralGameStateControllerInstance?.CurrentCampaignContext?.CurrentCombatContext == null)
            {
                foreach (Enemy key in new List<Enemy>(this.EnemyRepresenterUX.SpawnedEnemiesLookup.Keys))
                {
                    this.EnemyRepresenterUX.RemoveEnemy(key);
                }

                return;
            }

            foreach (Enemy curEnemy in new List<Enemy>(this.EnemyRepresenterUX.SpawnedEnemiesLookup.Keys))
            {
                if (!this.CurrentCampaignContext.CurrentCombatContext.Enemies.Contains(curEnemy))
                {
                    this.EnemyRepresenterUX.RemoveEnemy(curEnemy);
                }
            }
        }

        private void _RemoveDefeatedEntities()
        {
            if (this.CentralGameStateControllerInstance?.CurrentCampaignContext?.CurrentCombatContext == null)
            {
                foreach (Entity key in new List<Entity>(this.EnemyRepresenterUX._SpawnedEnemiesLookup.Keys))
                {
                    this.EnemyRepresenterUX._RemoveEnemy(key);
                }

                return;
            }

            foreach (Entity curEnemy in new List<Entity>(this.EnemyRepresenterUX._SpawnedEnemiesLookup.Keys))
            {
                if (!this.CurrentCampaignContext._CurrentEncounter.EncounterEntities.Contains(curEnemy))
                {
                    this.EnemyRepresenterUX._RemoveEnemy(curEnemy);
                }
            }
        }

        [Obsolete("Transition to " + nameof(_StatusEffectClicked))]
        public void StatusEffectClicked(SFDDCards.AppliedStatusEffect representingEffect)
        {
            if (representingEffect.BasedOnStatusEffect.WindowResponders.ContainsKey(KnownReactionWindows.Activated))
            {
                ReactionWindowContext activatedContext = new ReactionWindowContext(
                    this.CentralGameStateControllerInstance.CurrentCampaignContext,
                    KnownReactionWindows.Activated,
                    this.CentralGameStateControllerInstance.CurrentCampaignContext.CampaignPlayer, 
                    playedFromZone: "potion");

                if (representingEffect.TryGetReactionEvents(this.CentralGameStateControllerInstance.CurrentCampaignContext, activatedContext, out List<WindowResponse> responses))
                {
                    foreach (WindowResponse response in responses)
                    {
                        this.CentralGameStateControllerInstance.CurrentCampaignContext.IngestStatusEffectHappening(activatedContext, response);
                    }
                }
            }
        }

        public void _StatusEffectClicked(SpaceDeck.GameState.Minimum.AppliedStatusEffect representingEffect)
        {
            if (representingEffect.TriggerOnEventIds.Contains(WellknownGameStateEvents.Activated))
            {
                GameStateEventTrigger trigger = new GameStateEventTrigger(WellknownGameStateEvents.Activated);
                if (representingEffect.TryApplyStatusEffect(trigger, this.CentralGameStateControllerInstance.CurrentCampaignContext.GameplayState, out List<GameStateChange> changes))
                {
                    SpaceDeck.GameState.Execution.GameStateDelta delta = new SpaceDeck.GameState.Execution.GameStateDelta(this.CentralGameStateControllerInstance.CurrentCampaignContext.GameplayState, changes);
                    GameStateDeltaApplier.ApplyGameStateDelta(this.CentralGameStateControllerInstance.CurrentCampaignContext.GameplayState, delta);
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

            if (this.CentralGameStateControllerInstance?.CurrentCampaignContext?.CurrentCombatContext == null)
            {
                return;
            }

            this.CardBrowserUXInstance.gameObject.SetActive(true);
            this.CardBrowserUXInstance.SetLabelText("Now Viewing: Cards in Discard");
            this.CardBrowserUXInstance._SetFromCards(this.CentralGameStateControllerInstance.CurrentCampaignContext._CurrentEncounter.GetZoneCards(WellknownZones.Discard));
        }

        public void _OpenDeckCardsBrowser()
        {
            if (this.CardBrowserUXInstance.gameObject.activeInHierarchy)
            {
                GlobalUpdateUX.LogTextEvent.Invoke($"The card browser is already open. Perhaps you need to respond to an event.", GlobalUpdateUX.LogType.UserError);
                return;
            }

            if (this.CentralGameStateControllerInstance?.CurrentCampaignContext == null)
            {
                return;
            }

            List<CardInstance> cardsInDeck = new List<CardInstance>(this.CentralGameStateControllerInstance.CurrentCampaignContext.GameplayState.CardsInDeck);

            if (this.CentralGameStateControllerInstance?.CurrentCampaignContext?._CurrentEncounter != null)
            {
                cardsInDeck = new List<CardInstance>(this.CurrentCampaignContext._CurrentEncounter.GetZoneCards(WellknownZones.Deck));
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

            if (this.CentralGameStateControllerInstance?.CurrentCampaignContext?.CurrentCombatContext == null)
            {
                return;
            }

            List<CardInstance> cardsInExile = new List<CardInstance>(this.CentralGameStateControllerInstance.CurrentCampaignContext._CurrentEncounter.GetZoneCards(WellknownZones.Exile));
            cardsInExile.Sort((CardInstance a, CardInstance b) => a.Name.CompareTo(b.Name));

            this.CardBrowserUXInstance.gameObject.SetActive(true);
            this.CardBrowserUXInstance.SetLabelText("Now Viewing: Cards in Exile");
            this.CardBrowserUXInstance._SetFromCards(cardsInExile);
        }

        [Obsolete("Transition to " + nameof(_BeginHoverTarget))]
        public void BeginHoverTarget(ICombatantTarget target)
        {
            this.HoveredCombatant = target;
            this.PlayerHandRepresenter.ReactionWindowForSelectedCard = new ReactionWindowContext(
                this.CentralGameStateControllerInstance.CurrentCampaignContext,
                KnownReactionWindows.ConsideringPlayingFromHand,
                this.CentralGameStateControllerInstance.CurrentCampaignContext.CampaignPlayer,
                target,
                "hand");
        }

        public void _BeginHoverTarget(IChangeTarget target)
        {
            this._HoveredCombatant = target;
        }

        [Obsolete("Transition to " + nameof(_EndHoverTarget))]
        public void EndHoverTarget(ICombatantTarget target)
        {
            if (this.HoveredCombatant == target)
            {
                this.HoveredCombatant = null;
                this.PlayerHandRepresenter.ReactionWindowForSelectedCard = null;
            }
        }

        public void _EndHoverTarget(ICombatantTarget target)
        {
            if (this._HoveredCombatant == target)
            {
                this._HoveredCombatant = null;
            }
        }

        [Obsolete("Transition to the version of " + nameof(EncounterDialogueComplete) + " that uses EncounterState")]
        public void EncounterDialogueComplete(EvaluatedEncounter completed)
        {
            if (completed == this.CentralGameStateControllerInstance.CurrentCampaignContext.CurrentEncounter)
            {
                this.EncounterRepresenterUXInstance.Close();
                this.ProceedToNextRoom();
            }
        }

        public void EncounterDialogueComplete(EncounterState completed)
        {
            if (completed == this.CentralGameStateControllerInstance.CurrentCampaignContext._CurrentEncounter)
            {
                this.EncounterRepresenterUXInstance.Close();
                this.ProceedToNextRoom();
            }
        }
    }
}