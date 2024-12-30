namespace SpaceDeck.GameState.Minimum
{
    using SpaceDeck.Utility.Minimum;
    using System;
    using System.Collections.Generic;

    public class Reward
    {
        public enum RewardIdentityKind
        {
            Card = 0,
            Artifact = 1,
            Currency = 2
        }

        public readonly LowercaseString Id;
        public readonly RewardIdentityKind IdentityKind;

        public Reward(LowercaseString id, RewardIdentityKind identityKind)
        {
            this.Id = id;
            this.IdentityKind = identityKind;
        }

        public virtual int GetAmount(IGameStateMutator mutator)
        {
            return 1;
        }
    }
}