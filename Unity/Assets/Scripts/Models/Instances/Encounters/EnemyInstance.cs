namespace SpaceDeck.Models.Instances
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Models.Prototypes;
    using SpaceDeck.Utility.Minimum;

    public class EnemyInstance : Entity
    {
        public readonly LowercaseString Id;

        public EnemyInstance(EnemyPrototype prototype)
        {
            this.Id = prototype.Id;
        }
    }
}