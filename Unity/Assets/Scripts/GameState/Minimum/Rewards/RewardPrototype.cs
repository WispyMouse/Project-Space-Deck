namespace SpaceDeck.GameState.Minimum
{
    using SpaceDeck.Utility.Minimum;
    using System;
    using System.Collections.Generic;

    public class RewardPrototype
    {
        public enum RewardIdentityKind
        {
            Card = 0,
            Artifact = 1,
            Currency = 2
        }

        public readonly LowercaseString Id;
        public readonly RewardIdentityKind IdentityKind;
        public readonly int RewardAmount;

        public RewardPrototype(LowercaseString id, RewardIdentityKind identityKind, int rewardAmount = 1)
        {
            this.Id = id;
            this.IdentityKind = identityKind;
            this.RewardAmount = rewardAmount;
        }

        public virtual int GetAmount(IGameStateMutator mutator)
        {
            // TODO: This would theoretically allow you to do things like "whenever you gain a potion, gain another of it"
            // Will probably require some retooling, because "GetAmount" doesn't really communicate "GetAmountYouWouldGainIf..."
            return this.RewardAmount;
        }
    }
}