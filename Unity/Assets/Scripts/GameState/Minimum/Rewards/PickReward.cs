namespace SpaceDeck.GameState.Minimum
{
    using SpaceDeck.Utility.Minimum;
    using System;
    using System.Collections.Generic;

    public class PickReward
    {
        public enum PickRewardProtocol
        {
            ChooseX
        }

        public readonly PickRewardProtocol Protocol = PickRewardProtocol.ChooseX;
        public readonly int ProtocolArgument = 1;
        public readonly List<Reward> RewardOptions = new List<Reward>();

        public PickReward(PickRewardProtocol protocol, int protocolArgument, IEnumerable<Reward> rewardOptions)
        {
            this.Protocol = protocol;
            this.ProtocolArgument = protocolArgument;
            this.RewardOptions = new List<Reward>(rewardOptions);
        }
    }
}