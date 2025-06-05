namespace SpaceDeck.Models.Instances
{
    using System.Collections;
    using System.Collections.Generic;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.GameState.Deltas;
    using SpaceDeck.Models.Prototypes;
    using static SpaceDeck.GameState.Minimum.GameStateEventTrigger;
    using SpaceDeck.Utility.Logging;
    using SpaceDeck.Utility.Wellknown;
    using static SpaceDeck.GameState.Minimum.PickReward;

    public class LinkedPickReward : PickReward
    {
        public readonly List<LinkedRewardInstance> LinkedRewards = new List<LinkedRewardInstance>();

        public LinkedPickReward(PickReward pickReward, ILinkedPickRewardProvider linkedPickRewardProvider) : base(pickReward.Protocol, pickReward.ProtocolArgument, pickReward.RewardOptions)
        {
            foreach (RewardPrototype rewardPrototype in pickReward.RewardOptions)
            {
                this.LinkedRewards.Add(linkedPickRewardProvider.GetReward(rewardPrototype));
            }
        }
    }
}