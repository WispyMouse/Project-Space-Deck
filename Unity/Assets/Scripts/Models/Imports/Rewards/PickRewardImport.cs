namespace SpaceDeck.Models.Imports
{
    using SpaceDeck.GameState.Minimum;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using UnityEngine;
    using static SpaceDeck.GameState.Minimum.PickReward;
    using static SpaceDeck.GameState.Minimum.Reward;

    [System.Serializable]
    public class PickRewardImport
    {
        public PickRewardProtocol Protocol = PickRewardProtocol.ChooseX;
        public int ProtocolArgument = 1;

        public List<RewardIdentityImport> RewardIdentities = new List<RewardIdentityImport>();

        public PickReward GetRewards()
        {
            List<Reward> rewards = new List<Reward>();
            foreach (RewardIdentityImport identity in this.RewardIdentities)
            {
                // TODO: Somehow get rewards without access to databases???
            }
            return new PickReward(this.Protocol, this.ProtocolArgument, rewards);
        }
    }
}