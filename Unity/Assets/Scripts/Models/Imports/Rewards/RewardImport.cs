namespace SpaceDeck.Models.Imports
{
    using SpaceDeck.Models.Instances;
    using SpaceDeck.Models.Prototypes;
    using SpaceDeck.Utility.Minimum;
    using System;
    using System.Collections.Generic;

    [Serializable]

    public class RewardImport : Importable
    {
        public List<PickRewardImport> PickRewards { get; set; } = new List<PickRewardImport>();

        public RewardPrototype GetReward()
        {
            return new RewardPrototype(this.Id);
        }
    }
}