namespace SpaceDeck.Models.Prototypes
{
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Utility.Minimum;
    using System.Collections.Generic;

    public class PickRewardPrototype
    {
        public readonly LowercaseString Id;
        public readonly int PickNumber;
        public readonly IReadOnlyList<RewardPrototype> Rewards;

        public PickRewardPrototype(LowercaseString id, int pickNumber, IReadOnlyList<RewardPrototype> rewards)
        {
            this.Id = id;
            this.PickNumber = pickNumber;
            this.Rewards = rewards;
        }
    }
}