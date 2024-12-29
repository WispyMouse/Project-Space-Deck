namespace SFDDCards
{
    using SFDDCards.ImportModels;
    using SpaceDeck.Models.Imports;
    using SpaceDeck.Utility.Minimum;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using UnityEngine;

    public class Reward
    {
        public LowercaseString RewardId;

        public List<PickSomeReward> PickRewards = new List<PickSomeReward>();

        public Reward(RewardImport basedOn)
        {
            this.RewardId = basedOn.Id;
        }
    }
}