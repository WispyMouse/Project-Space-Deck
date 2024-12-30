namespace SpaceDeck.UX
{
    using SFDDCards;
    using SFDDCards.UX;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Tokenization.Minimum.Context;
    using SpaceDeck.UX;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using UnityEngine;


    public class CombatTurnController : MonoBehaviour, ICombatTurnController
    {
        private static CombatTurnController Instance { get; set; }

        [SerializeReference]
        private CentralGameStateController CentralGameStateControllerInstance;

        public CampaignContext ForCampaign => this.CentralGameStateControllerInstance?.CurrentCampaignContext;

        [SerializeReference]
        private GameplayUXController UXController;
        [SerializeReference]
        private EnemyRepresenterUX EnemyRepresenterUX;

        [Obsolete("Transition to " + nameof(_Context))]
        private CombatContext Context => this.CentralGameStateControllerInstance?.CurrentCampaignContext?.CurrentCombatContext;
        private EncounterState _Context => this.CentralGameStateControllerInstance?.CurrentCampaignContext?._CurrentEncounter;

        private QuestionAnsweringContext CurrentQuestionAnsweringContext { get; set; }
        
        public bool CurrentlyActive { get; private set; } = false;

        private static Coroutine AnimationCoroutine { get; set; } = null;
        private static bool AnimationCoroutineIsRunning { get; set; } = false;

        private void Awake()
        {
            if (Instance != null)
            {
                this.enabled = false;
                return;
            }

            Instance = this;

            GlobalSequenceEventHolder.OnStopAllSequences.AddListener(StopSequenceAnimation);
        }

        [Obsolete("Transition to " + nameof(_BeginHandlingCombat))]
        public void BeginHandlingCombat()
        {
            this.CurrentlyActive = true;

            GlobalSequenceEventHolder.PushSequencesToTop(
                CentralGameStateControllerInstance.CurrentCampaignContext,
                new GameplaySequenceEvent(this._SpawnInitialEnemies, null),
                new GameplaySequenceEvent(() => this.Context.EndCurrentTurnAndChangeTurn(CombatContext.TurnStatus.PlayerTurn), null)
                );
        }

        public void _BeginHandlingCombat()
        {
            this.CurrentlyActive = true;
            this._SpawnInitialEnemies();
        }

        public void EndHandlingCombat()
        {
            this.CurrentlyActive = false;
            GlobalSequenceEventHolder.StopAllSequences();
        }

        private void StopSequenceAnimation()
        {
            if (AnimationCoroutine != null)
            {
                Instance.StopCoroutine(AnimationCoroutine);
                AnimationCoroutine = null;
            }
        }

        #region Sequence Resolution

        private void Update()
        {
            do
            {
                if (GlobalSequenceEventHolder.StackedSequenceEvents.Count == 0 && GlobalSequenceEventHolder.CurrentSequenceEvent == null)
                {
                    return;
                }

                if (AnimationCoroutineIsRunning)
                {
                    return;
                }

                GlobalSequenceEventHolder.ApplyNextSequenceWithAnimationHandler(this);
            }
            while (GlobalSequenceEventHolder.StackedSequenceEvents.Count > 0 && !GlobalUpdateUX.PendingPlayerChoice);
        }

        public void HandleSequenceEventWithAnimation(GameplaySequenceEvent sequenceEvent)
        {
            AnimationCoroutine = this.StartCoroutine(AnimateHandleSequenceEventWithAnimation(sequenceEvent));
        }

        private IEnumerator AnimateHandleSequenceEventWithAnimation(GameplaySequenceEvent runningEvent)
        {
            AnimationCoroutineIsRunning = true;

            yield return runningEvent.AnimationDelegate();
            runningEvent.ConsequentialAction?.Invoke();

            GlobalUpdateUX.UpdateUXEvent.Invoke(this.CentralGameStateControllerInstance.CurrentCampaignContext);

            AnimationCoroutineIsRunning = false;
            AnimationCoroutine = null;
        }

        #endregion

        #region Specific Gameplay Turn Concepts
        
        private void _SpawnInitialEnemies()
        {
            this.EnemyRepresenterUX._AddEnemies(this.CentralGameStateControllerInstance.CurrentCampaignContext._CurrentEncounter.EncounterEntities);
        }

        private void _SpawnEnemy(Entity toSpawn)
        {
            this.EnemyRepresenterUX._AddEnemy(toSpawn);
        }

        public void _EndPlayerTurn()
        {
            this.CentralGameStateControllerInstance.CurrentCampaignContext.GameplayState.EndCurrentEntityTurn();
        }

        public void _StartPlayCard(CardInstance toPlay)
        {
            QuestionAnsweringContext questionContext = this.CentralGameStateControllerInstance.CurrentCampaignContext.GameplayState.StartConsideringPlayingCard(toPlay);
            this.CurrentQuestionAnsweringContext = questionContext;
            if (this.CentralGameStateControllerInstance.CurrentCampaignContext.GameplayState.TryExecuteCurrentCard())
            {
                // The card has been played, hooray
                this.CurrentQuestionAnsweringContext = null;
                return;
            }
            // The card has questions to answer!
            // TODO: Answer these questions, then execute card
        }

        public void _ExecuteCurrentCard()
        {
            if (!this.CentralGameStateControllerInstance.CurrentCampaignContext.GameplayState.TryExecuteCurrentCard())
            {
                // TODO LOG
                // Should not fail to play cards if this is directly called
                return;
            }
        }

        #endregion
    }
}