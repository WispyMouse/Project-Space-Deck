namespace SpaceDeck.Models.Databases
{
    using System.Collections.Generic;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Models.Instances;
    using SpaceDeck.Models.Prototypes;
    using SpaceDeck.Utility.Logging;
    using SpaceDeck.Utility.Minimum;
    using SpaceDeck.Utility.Wellknown;

    public class RewardProvider : IPickRewardProvider
    {
        public static readonly RewardProvider Instance = new RewardProvider();

        private RewardProvider()
        {

        }

        public IEnumerable<PickReward> GetRewards(IEnumerable<PickRewardPrototype> rewards, RandomDecider<LowercaseString> decider = null)
        {
            if (decider == null)
            {
                decider = new RandomDecider<LowercaseString>();
            }

            List<PickReward> pickRewards = new List<PickReward>();

            foreach (PickRewardPrototype prototype in rewards)
            {
                pickRewards.Add(RewardDatabase.GetReward(prototype.Id));
            }

            return pickRewards;
        }
    }
}