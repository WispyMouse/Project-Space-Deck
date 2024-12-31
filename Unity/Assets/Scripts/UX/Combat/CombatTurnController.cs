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


    public class CombatTurnController : MonoBehaviour
    {
        private static CombatTurnController Instance { get; set; }

        [SerializeReference]
        private CentralGameStateController CentralGameStateControllerInstance;

        [SerializeReference]
        private GameplayUXController UXController;
        [SerializeReference]
        private EnemyRepresenterUX EnemyRepresenterUX;

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
        }

        public void _BeginHandlingCombat()
        {
            this.CurrentlyActive = true;
            this._SpawnInitialEnemies();
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

        #region Specific Gameplay Turn Concepts
        
        private void _SpawnInitialEnemies()
        {
            this.EnemyRepresenterUX._AddEnemies(this.CentralGameStateControllerInstance.GameplayState.CurrentEncounterState.EncounterEntities);
        }

        private void _SpawnEnemy(Entity toSpawn)
        {
            this.EnemyRepresenterUX._AddEnemy(toSpawn);
        }

        public void _EndPlayerTurn()
        {
            this.CentralGameStateControllerInstance.GameplayState.EndCurrentEntityTurn();
        }

        public void _StartPlayCard(CardInstance toPlay)
        {
            QuestionAnsweringContext questionContext = this.CentralGameStateControllerInstance.GameplayState.StartConsideringPlayingCard(toPlay);
            this.CurrentQuestionAnsweringContext = questionContext;
            if (this.CentralGameStateControllerInstance.GameplayState.TryExecuteCurrentCard())
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
            if (!this.CentralGameStateControllerInstance.GameplayState.TryExecuteCurrentCard())
            {
                // TODO LOG
                // Should not fail to play cards if this is directly called
                return;
            }
        }

        #endregion
    }
}