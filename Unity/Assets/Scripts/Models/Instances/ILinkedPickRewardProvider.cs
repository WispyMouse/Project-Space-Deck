namespace SpaceDeck.Models.Instances
{
    using System.Collections.Generic;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Models.Prototypes;
    using SpaceDeck.Utility.Minimum;

    public interface ILinkedPickRewardProvider
    {
        IEnumerable<LinkedPickReward> GetRewards(IEnumerable<LowercaseString> ids, RandomDecider<LowercaseString> decider = null);
        IEnumerable<LinkedPickReward> GetRewards(IEnumerable<PickRewardPrototype> prototypes, RandomDecider<LowercaseString> decider = null);
        LinkedRewardInstance GetReward(RewardPrototype rewardPrototype, RandomDecider<LowercaseString> decider = null);
        LinkedShopEntry GetShopEntry(LowercaseStringSet criteria);
    }
}