namespace SpaceDeck.Models.Instances
{
    using System.Collections.Generic;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Models.Prototypes;
    using SpaceDeck.Utility.Minimum;

    public interface IPickRewardProvider
    {
        IEnumerable<PickReward> GetRewards(IEnumerable<PickRewardPrototype> rewards, RandomDecider<LowercaseString> decider = null);
    }
}