namespace SpaceDeck.Models.Imports
{
    using SpaceDeck.Models.Instances;
    using SpaceDeck.Models.Prototypes;
    using SpaceDeck.Utility.Minimum;
    using System;
    using System.Collections.Generic;
    using static SpaceDeck.GameState.Minimum.Reward;

    [Serializable]

    public class RewardImport : Importable
    {
        public readonly RewardIdentityKind IdentityKind;

        public RewardPrototype GetReward()
        {
            return new RewardPrototype(this.Id, this.IdentityKind);
        }
    }
}