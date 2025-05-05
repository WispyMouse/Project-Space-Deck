namespace SpaceDeck.Models.Instances
{
    using System.Collections.Generic;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Models.Prototypes;
    using SpaceDeck.Utility.Minimum;

    public interface IPickRewardProvider
    {
        IEnumerable<PickReward> GetRewards(IEnumerable<LowercaseString> ids, RandomDecider<LowercaseString> decider = null);
        IEnumerable<PickReward> GetRewards(IEnumerable<PickRewardPrototype> prototypes, RandomDecider<LowercaseString> decider = null);
    }
}