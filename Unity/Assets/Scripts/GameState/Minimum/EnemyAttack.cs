namespace SpaceDeck.GameState.Minimum
{
    using SpaceDeck.Utility.Minimum;
    using System.Collections.Generic;

    public class EnemyAttack : Intent
    {
        public LowercaseString Id;
        public LowercaseString RawAttackScript;

        public EnemyAttack(LowercaseString id, LowercaseString rawAttackScript)
        {
            this.Id = id;
            this.RawAttackScript = rawAttackScript;
        }
    }
}