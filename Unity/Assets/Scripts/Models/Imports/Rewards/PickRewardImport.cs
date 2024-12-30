namespace SpaceDeck.Models.Imports
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using UnityEngine;
    using static SpaceDeck.GameState.Minimum.Reward;

    [System.Serializable]
    public class PickRewardImport
    {
        public PickRewardProtocol Protocol = PickRewardProtocol.ChooseX;
        public int ProtocolArgument = 1;

        public List<RewardIdentity> RewardIdentities = new List<RewardIdentity>();
    }
}