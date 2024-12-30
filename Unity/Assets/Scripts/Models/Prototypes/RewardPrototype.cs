namespace SpaceDeck.Models.Prototypes
{
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.Utility.Minimum;
    using System.Collections;
    using System.Collections.Generic;

    public class RewardPrototype
    {
        public readonly LowercaseString Id;

        public RewardPrototype(LowercaseString id)
        {
            this.Id = id;
        }
    }
}