namespace SpaceDeck.Models.Instances
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.GameState.Deltas;
    using SpaceDeck.Models.Prototypes;
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.Utility.Logging;
    using SpaceDeck.Utility.Minimum;
    using SpaceDeck.Utility.Wellknown;

    public class LinkedEnemyAttack : EnemyAttack
    {
        public LinkedTokenList LinkedAttackScript;

        public LinkedEnemyAttack(EnemyAttack baseAttack, LinkedTokenList linkedTokenList) : base(baseAttack.Id, baseAttack.RawAttackScript)
        {
            this.LinkedAttackScript = linkedTokenList;
        }

        public override IReadOnlyList<GameStateChange> ActOnIntent(IGameStateMutator mutator)
        {
            if (!GameStateDeltaMaker.TryCreateDelta(this.LinkedAttackScript, mutator, out GameStateDelta delta))
            {
                Logging.DebugLog(WellknownLoggingLevels.Error, WellknownLoggingCategories.TryCreateDelta, "Failed to create delta for linked enemy attack.");
            }

            return delta.Changes;
        }
    }
}