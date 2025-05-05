namespace SpaceDeck.Models.Databases
{
    using System.Collections.Generic;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Models.Instances;
    using SpaceDeck.Models.Prototypes;
    using SpaceDeck.Utility.Logging;
    using SpaceDeck.Utility.Minimum;
    using SpaceDeck.Utility.Wellknown;

    public class RewardDatabasePickRewardProvider : IPickRewardProvider
    {
        public static readonly RewardDatabasePickRewardProvider Instance = new RewardDatabasePickRewardProvider();

        private RewardDatabasePickRewardProvider()
        {

        }

        public IEnumerable<PickReward> GetRewards(IEnumerable<LowercaseString> ids, RandomDecider<LowercaseString> decider = null)
        {
            List<PickReward> pickRewards = new List<PickReward>();

            foreach (LowercaseString reward in ids)
            {
                pickRewards.Add(RewardDatabase.GetReward(reward));
            }

            return pickRewards;
        }

        public IEnumerable<PickReward> GetRewards(IEnumerable<PickRewardPrototype> rewards, RandomDecider<LowercaseString> decider = null)
        {
            List<PickReward> pickRewards = new List<PickReward>();

            foreach (PickRewardPrototype reward in rewards)
            {
                pickRewards.Add(RewardDatabase.GetReward(reward));
            }

            return pickRewards;
        }
    }
}