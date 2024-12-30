namespace SpaceDeck.Models.Prototypes
{
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.Utility.Minimum;
    using System.Collections;
    using System.Collections.Generic;
    using static SpaceDeck.GameState.Minimum.Reward;

    public class RewardPrototype
    {
        public readonly LowercaseString Id;

        public readonly RewardIdentityKind IdentityKind;

        public RewardPrototype(LowercaseString id, RewardIdentityKind identityKind)
        {
            this.Id = id;
            this.IdentityKind = identityKind;
        }
    }
}