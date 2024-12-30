namespace SFDDCards
{
    using SFDDCards.ImportModels;
    using SpaceDeck.Models.Imports;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using UnityEngine;
    using static SpaceDeck.GameState.Minimum.Reward;
    using static SpaceDeck.Models.Imports.PickRewardImport;

    public class PickSomeReward
    {
        public readonly PickRewardImport BasedOn;

        public PickRewardProtocol Protocol => this.BasedOn.Protocol;
        public List<PickSomeRewardSlot> PickRewardSlots = new List<PickSomeRewardSlot>();

        public PickSomeReward(PickRewardImport basedOn)
        {
            this.BasedOn = basedOn;
        }
    }
}