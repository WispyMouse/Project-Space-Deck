namespace SpaceDeck.GameState.Minimum
{
    using SpaceDeck.Utility.Minimum;
    using System.Collections.Generic;

    public class EnemyAttack : Intent
    {
        public readonly LowercaseString Id;
        public readonly LowercaseString RawAttackScript;

        public EnemyAttack(LowercaseString id, LowercaseString rawAttackScript)
        {
            this.Id = id;
            this.RawAttackScript = rawAttackScript;
        }
    }
}