namespace SpaceDeck.Models.Databases
{
    using System.Collections.Generic;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Models.Instances;
    using SpaceDeck.Models.Prototypes;
    using SpaceDeck.Utility.Logging;
    using SpaceDeck.Utility.Minimum;
    using SpaceDeck.Utility.Wellknown;

    public class RewardDatabasePickRewardProvider : ILinkedPickRewardProvider
    {
        public static readonly RewardDatabasePickRewardProvider Instance = new RewardDatabasePickRewardProvider();

        private RewardDatabasePickRewardProvider()
        {

        }

        public IEnumerable<LinkedPickReward> GetRewards(IEnumerable<LowercaseString> ids, RandomDecider<LowercaseString> decider = null)
        {
            List<PickRewardPrototype> rewardPrototypes = new List<PickRewardPrototype>();
            foreach (LowercaseString reward in ids)
            {
                PickRewardPrototype pickReward = RewardDatabase.GetPickRewardPrototype(reward);
                rewardPrototypes.Add(pickReward);
            }

            return this.GetRewards(rewardPrototypes, decider);
        }

        public IEnumerable<LinkedPickReward> GetRewards(IEnumerable<PickRewardPrototype> rewards, RandomDecider<LowercaseString> decider = null)
        {
            List<LinkedPickReward> pickRewards = new List<LinkedPickReward>();

            foreach (PickRewardPrototype reward in rewards)
            {
                PickReward pickReward = RewardDatabase.GetPickReward(reward);
                LinkedPickReward linkedPickReward = new LinkedPickReward(pickReward, this);
                pickRewards.Add(linkedPickReward);
            }

            return pickRewards;
        }

        public LinkedRewardInstance GetReward(RewardPrototype rewardPrototype, RandomDecider<LowercaseString> decider = null)
        {
            return RewardDatabase.GetReward(rewardPrototype);
        }
    }
}