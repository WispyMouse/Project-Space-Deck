namespace SpaceDeck.Models.Prototypes
{
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.Utility.Minimum;
    using System.Collections;
    using System.Collections.Generic;

    public class EnemyPrototype
    {
        public LowercaseString Id;

        public EnemyPrototype(LowercaseString id)
        {
            this.Id = id;
        }
    }
}