namespace SpaceDeck.UX
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;
    using System.Text.RegularExpressions;
    using UnityEngine;
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

        private Entity previousCombatTurnTaker { get; set; } = null;
        public GameState CurrentGameState => this.CentralGameStateControllerInstance?.GameplayState;
        public EncounterState CurrentEncounterState => this.CurrentGameState.CurrentEncounterState;

        public IChangeTarget HoveredCombatant { get; set; } = null;

        private void Awake()
        {
            this.Annihilate();
        }

        private void OnEnable()
        {
        }

        private void OnDestroy()
        {
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(1))
            {
                this.CancelAllSelections();
            }
        }

        public PlayerUX PlacePlayerCharacter()
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

            this.PlayerUXInstance.SetFromPlayer(campaignEntity);

            this.LifeValue.text = $"{campaignEntity.Qualities.GetNumericQuality(WellknownQualities.Health, 0).ToString()} / {campaignEntity.Qualities.GetNumericQuality(WellknownQualities.MaximumHealth, 0).ToString()}";
            return this.PlayerUXInstance;
        }

        public void CheckAndActIfGameCampaignNavigationStateChanged()
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

            if (this.CurrentGameState.PendingRewards != null && this.RewardsPanelUXInstance.Rewards != this.CurrentGameState.PendingRewards)
            {
                this.PresentAwards(this.CurrentGameState.PendingRewards);
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
                        this.EncounterRepresenterUXInstance.RepresentEncounter(this.CurrentCampaignContext._CurrentEncounter, this.CurrentCampaignContext.GameplayState);
                    }
                    else if (this.CurrentCampaignContext._CurrentEncounter.IsShopEncounter)
                    {
                        this.EncounterRepresenterUXInstance.Close();
                        this.ShowShopPanel(this.CurrentCampaignContext._CurrentEncounter.GetShop());
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
            */
        }

        public void UpdateUX()
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

        public void SelectTarget(IChangeTarget toSelect)
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

        public void SelectCurrentCard(DisplayedCardUX toSelect)
        {
            if (this.CentralGameStateControllerInstance.GameplayState.CurrentEncounterState == null)
            {
                return;
            }

            if (this.PlayerIsCurrentlyAnimating)
            {
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

        public void ShowRewardsPanel(List<PickReward> toReward)
        {
            this.RewardsPanelUXInstance.gameObject.SetActive(true);
            this.RewardsPanelUXInstance.SetReward(toReward);
            this.UpdateUX();
        }

        public void ShowShopPanel(IReadOnlyList<IShopEntry> itemsInShop)
        {
            this.ShopPanelUXInstance.gameObject.SetActive(true);
            this.ShopPanelUXInstance.SetShopItems(itemsInShop);
        }

        private IEnumerator AnimateEnemyTurnsInternal(Action continuationAction)
        {
            throw new NotImplementedException();
        }

        public IEnumerator AnimateCardPlay(CardInstance toPlay, IChangeTarget target)
        {
            yield return AnimateCardPlayInternal(toPlay, target);
        }

        private IEnumerator AnimateCardPlayInternal(CardInstance toPlay, IChangeTarget target)
        {
            PlayerIsCurrentlyAnimating = true;

            // TODO: REINTRODUCE CARD ANIMATIONS
            yield return AnimateAction(this.PlayerUXInstance, target);

            PlayerIsCurrentlyAnimating = false;
        }

        private IEnumerator AnimateAction(IAnimationPuppet puppet, IChangeTarget target)
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
                    targetPuppet = this.EnemyRepresenterUX.SpawnedEnemiesLookup[entity];
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
            if (this.CentralGameStateControllerInstance?.CampaignPlayer == null)
            {
                this.LifeValue.text = "0";
                this.PlayerStatusEffectUXHolderInstance.Annihilate();

                return;
            }

            this.LifeValue.text = this.CentralGameStateControllerInstance.CampaignPlayer.Qualities.GetNumericQuality(WellknownQualities.Health).ToString();
            this.PlayerStatusEffectUXHolderInstance.SetStatusEffects(
                this.CentralGameStateControllerInstance.CampaignPlayer.AppliedStatusEffects.Values,
                this.StatusEffectClicked);
        }

        private void SetElementValueLabel()
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

        private void SetCurrenciesValueLabel()
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

        private void AppointTargetableIndicatorsToValidTargets(CardInstance toTarget)
        {
            this.ClearAllTargetableIndicators();

            List<IChangeTarget> remainingTargets = new List<IChangeTarget>(toTarget.GetPossibleTargets(this.CentralGameStateControllerInstance.GameplayState));

            if (remainingTargets.Count > 0)
            {
                // TODO: is this the all foes target?
                foreach (IChangeTarget target in remainingTargets)
                {
                    if (target == NobodyTarget.Instance)
                    {
                        this.NoTargetsIndicator.SetFromTarget(target, SelectTarget, BeginHoverTarget, EndHoverTarget);
                        this.NoTargetsIndicator.gameObject.SetActive(true);
                    }
                    else
                    {
                        IReadOnlyList<Entity> representedEntities = new List<Entity>(target.GetRepresentedEntities(this.CentralGameStateControllerInstance.GameplayState));

                        foreach (Entity representedEntity in representedEntities)
                        {
                            if (representedEntity == this.CentralGameStateControllerInstance.CampaignPlayer)
                            {
                                PlayerUX playerUx = this.PlayerUXInstance;
                                TargetableIndicator playerIndicator = Instantiate(this.SingleCombatantTargetableIndicatorPF, playerUx.transform);
                                playerIndicator.SetFromTarget(representedEntities[0], this.SelectTarget, BeginHoverTarget, EndHoverTarget);
                                this.ActiveIndicators.Add(playerIndicator);
                            }
                            else
                            {
                                if (this.EnemyRepresenterUX.SpawnedEnemiesLookup.TryGetValue(representedEntity, out EnemyUX enemyUX))
                                {
                                    TargetableIndicator playerIndicator = Instantiate(this.SingleCombatantTargetableIndicatorPF, enemyUX.transform);
                                    playerIndicator.SetFromTarget(representedEntities[0], this.SelectTarget, BeginHoverTarget, EndHoverTarget);
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
            if (this.CurrentEncounterState == null || this.CurrentEncounterState.EncounterEntities != null)
            {
                return;
            }

            foreach (Entity curEnemy in this.CurrentEncounterState.EncounterEntities)
            {
                if (!this.EnemyRepresenterUX.SpawnedEnemiesLookup.TryGetValue(curEnemy, out EnemyUX value))
                {
                    // It could be that the UpdateUX call was made before these enemies are spawned in to the game
                    // In that case, just continue
                    continue;
                }

                value.UpdateUX(this.CentralGameStateControllerInstance);
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

            this.AppointTargetableIndicatorsToValidTargets(this.CurrentSelectedCard.RepresentedCard);
        }

        public void EndTurn()
        {
            this.CancelAllSelections();

            if (this.CentralGameStateControllerInstance.GameplayState.EntityTurnTakerCalculator.TryGetCurrentEntityTurn(this.CurrentGameState, out Entity currentTurnEntity) && currentTurnEntity != this.PlayerUXInstance.RepresentedPlayer)
            {
                return;
            }

            this.CombatTurnCounterInstance.EndPlayerTurn();
        }

        public void PresentAwards(List<PickReward> toPresent)
        {
            this.CurrentGameState.PendingRewards = null;
            this.CancelAllSelections();
            this.ShowRewardsPanel(toPresent);
        }

        void PresentNextRouteChoice()
        {
            this.CancelAllSelections();

            SpaceDeck.GameState.Minimum.ChoiceNode campaignNode = this.CurrentGameState.GetCampaignCurrentNode();

            if (campaignNode == null)
            {
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

        public void NodeIsChosen(SpaceDeck.GameState.Minimum.ChoiceNodeOption option)
        {
            this.CancelAllSelections();
            this.CentralGameStateControllerInstance.GameplayState.MakeChoiceNodeDecision(option);
        }

        public void ProceedToNextRoom()
        {
            this.CancelAllSelections();
            if (this.CentralGameStateControllerInstance.GameplayState.StartNextRoomFromCampaign(out SpaceDeck.GameState.Minimum.ChoiceNode nextChoice))
            {
                this.PresentNextRouteChoice();
            }
        }

        public void RouteChosen(Route chosenRoute)
        {
            this.CancelAllSelections();
            this.CentralGameStateControllerInstance.RouteChosen(chosenRoute);
        }

        public void ShowCampaignChooser()
        {
            this.CampaignChooserUXInstance.ShowChooser();
        }

        private void RemoveDefeatedEntities()
        {
            if (this.CentralGameStateControllerInstance?.GameplayState.CurrentEncounterState == null)
            {
                foreach (Entity key in new List<Entity>(this.EnemyRepresenterUX.SpawnedEnemiesLookup.Keys))
                {
                    this.EnemyRepresenterUX.RemoveEnemy(key);
                }

                return;
            }

            foreach (Entity curEnemy in new List<Entity>(this.EnemyRepresenterUX.SpawnedEnemiesLookup.Keys))
            {
                if (!this.CurrentEncounterState.EncounterEntities.Contains(curEnemy))
                {
                    this.EnemyRepresenterUX.RemoveEnemy(curEnemy);
                }
            }
        }

        public void StatusEffectClicked(SpaceDeck.GameState.Minimum.AppliedStatusEffect representingEffect)
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

        public void OpenAllCardsBrowser()
        {
            if (this.CardBrowserUXInstance.gameObject.activeInHierarchy)
            {
                return;
            }

            this.CardBrowserUXInstance.gameObject.SetActive(true);
            this.CardBrowserUXInstance.SetLabelText("Now Viewing: All Cards");
            this.CardBrowserUXInstance.SetFromCards(SpaceDeck.Models.Databases.CardDatabase.GetOneOfEveryCard());
        }

        public void OpenDiscardCardsBrowser()
        {
            if (this.CardBrowserUXInstance.gameObject.activeInHierarchy)
            {
                return;
            }

            if (this.CentralGameStateControllerInstance?.GameplayState?.CurrentEncounterState == null)
            {
                return;
            }

            this.CardBrowserUXInstance.gameObject.SetActive(true);
            this.CardBrowserUXInstance.SetLabelText("Now Viewing: Cards in Discard");
            this.CardBrowserUXInstance.SetFromCards(this.CentralGameStateControllerInstance.GameplayState.CurrentEncounterState.GetZoneCards(WellknownZones.Discard));
        }

        public void OpenDeckCardsBrowser()
        {
            if (this.CardBrowserUXInstance.gameObject.activeInHierarchy)
            {
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
            this.CardBrowserUXInstance.SetFromCards(cardsInDeck);
        }

        public void OpenExileCardsBrowser()
        {
            if (this.CardBrowserUXInstance.gameObject.activeInHierarchy)
            {
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
            this.CardBrowserUXInstance.SetFromCards(cardsInExile);
        }

        public void BeginHoverTarget(IChangeTarget target)
        {
            this.HoveredCombatant = target;
        }

        public void EndHoverTarget(IChangeTarget target)
        {
            if (this.HoveredCombatant == target)
            {
                this.HoveredCombatant = null;
            }
        }

        public void EncounterDialogueComplete(EncounterState completed)
        {
            if (completed == this.CentralGameStateControllerInstance.GameplayState.CurrentEncounterState)
            {
                this.EncounterRepresenterUXInstance.Close();
                this.ProceedToNextRoom();
            }
        }
    }
}