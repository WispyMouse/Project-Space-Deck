namespace SpaceDeck.Models.Imports
{
    using SpaceDeck.Utility.Minimum;
    using System;
    using System.Collections.Generic;

    [Serializable]

    public class RewardImport : Importable
    {
        public LowercaseString Id;
        public List<PickRewardImport> PickRewards { get; set; } = new List<PickRewardImport>();
    }
}