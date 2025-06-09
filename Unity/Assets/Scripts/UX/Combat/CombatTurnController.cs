namespace SpaceDeck.UX
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using UnityEngine;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Tokenization.Minimum.Context;
    using SpaceDeck.UX;
    using SpaceDeck.GameState.Execution;
    using SpaceDeck.Utility.Wellknown;
    using SpaceDeck.Utility.Logging;
    using SpaceDeck.Tokenization.Evaluatables.Questions;

    public class CombatTurnController : MonoBehaviour
    {
        private static CombatTurnController Instance { get; set; }

        [SerializeReference]
        private CentralGameStateController CentralGameStateControllerInstance;

        [SerializeReference]
        private GameplayUXController UXController;
        [SerializeReference]
        private EnemyRepresenterUX EnemyRepresenterUX;


        [SerializeReference]
        private TargetableIndicator SingleCombatantTargetableIndicatorPF;
        private List<TargetableIndicator> ActiveIndicators { get; set; } = new List<TargetableIndicator>();
        [SerializeReference]
        private TargetableIndicator NoTargetsIndicator;
        [SerializeReference]
        private TargetableIndicator AllFoeTargetsIndicator;

        private GameState GameplayState => this.CentralGameStateControllerInstance?.GameplayState;
        
        public bool CurrentlyActive { get; private set; } = false;

        private static Coroutine AnimationCoroutine { get; set; } = null;
        private static bool AnimationCoroutineIsRunning { get; set; } = false;

        public IChangeTarget HoveredCombatant { get; set; } = null;

        private void Awake()
        {
            if (Instance != null)
            {
                this.enabled = false;
                return;
            }

            Instance = this;
        }

        public void BeginHandlingCombat()
        {
            this.CurrentlyActive = true;
            this.SpawnInitialEnemies();
        }

        public void EndHandlingCombat()
        {
            this.CurrentlyActive = false;
        }

        private void StopSequenceAnimation()
        {
            if (AnimationCoroutine != null)
            {
                Instance.StopCoroutine(AnimationCoroutine);
                AnimationCoroutine = null;
            }
        }

        public void ClearAllTargetableIndicators()
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

        #region Specific Gameplay Turn Concepts

        private void SpawnInitialEnemies()
        {
            this.EnemyRepresenterUX.AddEnemies(this.GameplayState.CurrentEncounterState.EncounterEntities);
        }

        public void EndPlayerTurn()
        {
            if (this.GameplayState.CurrentCampaignState != WellknownCampaignStates.CombatEncounter)
            {
                // Do nothing if not in a combat encounter
                Logging.DebugLog(WellknownLoggingLevels.Warning, WellknownLoggingCategories.GameplayUXController, $"Asked to end turn, but it's not a combat.");
                return;
            }

            this.GameplayState.EndCurrentEntityTurn();
            PendingResolveExecutor.ResolveAll(this.GameplayState);
        }

        public void StartPlayCard(CardInstance toPlay)
        {
            if (this.GameplayState.CurrentCampaignState != WellknownCampaignStates.CombatEncounter)
            {
                // Do nothing if not in a combat encounter
                return;
            }

            this.GameplayState.StartConsideringPlayingCard(toPlay);
            this.RepresentCardQuestions(toPlay);
        }

        public void ExecuteCurrentCard(ExecutionAnswerSet answers)
        {
            if (this.GameplayState.CurrentCampaignState != WellknownCampaignStates.CombatEncounter)
            {
                // Do nothing if not in a combat encounter
                return;
            }

            this.ClearAllTargetableIndicators();

            if (!this.GameplayState.TryExecuteCurrentCard(answers))
            {
                // TODO LOG
                // Should not fail to play cards if this is directly called
                return;
            }
        }

        public void RepresentCardQuestions(CardInstance toRepresent)
        {
            IReadOnlyList<ExecutionQuestion> questions = toRepresent.GetQuestions();

            if (questions.Count == 0)
            {
                Logging.DebugLog(WellknownLoggingLevels.Debug, WellknownLoggingCategories.GameplayUXController, "Asked to represent a card with no questions.");
                return;
            }

            foreach (ExecutionQuestion curQuestion in questions)
            {
                if (curQuestion is EffectTargetExecutionQuestion targetQuestion)
                {
                    IReadOnlyList<IChangeTarget> targets = targetQuestion.Options.GetProvidedTargets(this.GameplayState);
                    this.AppointTargetableIndicatorsToValidTargets(toRepresent, targetQuestion, targets);

                    // HACK: Handle exactly one target case, and ignore others
                    return;
                }
            }
        }

        private void AppointTargetableIndicatorsToValidTargets(CardInstance toTarget, EffectTargetExecutionQuestion question, IReadOnlyList<IChangeTarget> targets)
        {
            this.ClearAllTargetableIndicators();

            List<IChangeTarget> remainingTargets = new List<IChangeTarget>(targets);

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
                                PlayerUX playerUx = this.UXController.PlayerUXInstance;
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

        public void SelectTarget(IChangeTarget toSelect)
        {
            Debug.Log("This was hit once");
            if (this.CentralGameStateControllerInstance.GameplayState.CurrentlyConsideredPlayedCard == null)
            {
                Logging.DebugLog(WellknownLoggingLevels.Debug, WellknownLoggingCategories.GameplayUXController, $"There isn't a currently considered card, so can't respond to target selection.");
                return;
            }

            if (!this.CentralGameStateControllerInstance.GameplayState.EntityTurnTakerCalculator.TryGetCurrentEntityTurn(this.GameplayState, out Entity currentUser))
            {
                Logging.DebugLog(WellknownLoggingLevels.Debug, WellknownLoggingCategories.GameplayUXController, $"There isn't an entity taking a turn, so nothing can play the card.");
                return;
            }

            IReadOnlyList<ExecutionQuestion> questions = this.CentralGameStateControllerInstance.GameplayState.CurrentlyConsideredPlayedCard.GetQuestions();
            EffectTargetExecutionAnswer answer = new EffectTargetExecutionAnswer(questions[0], toSelect);
            ExecutionAnswerSet answers = new ExecutionAnswerSet(answer, currentUser);
            this.ExecuteCurrentCard(answers);
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

        #endregion
    }
}