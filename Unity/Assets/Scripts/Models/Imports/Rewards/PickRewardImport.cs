namespace SpaceDeck.Models.Imports
{
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Models.Prototypes;
    using SpaceDeck.Utility.Logging;
    using SpaceDeck.Utility.Wellknown;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using UnityEngine;
    using static SpaceDeck.GameState.Minimum.PickReward;
    using static SpaceDeck.GameState.Minimum.Reward;

    [System.Serializable]
    public class PickRewardImport : Importable
    {
        public PickRewardProtocol Protocol = PickRewardProtocol.ChooseX;
        public int ProtocolArgument = 1;

        public List<RewardIdentityImport> RewardIdentities = new List<RewardIdentityImport>();

        public PickRewardPrototype GetPrototype()
        {
            List<Reward> rewardIdentities = new List<Reward>();
            foreach (RewardIdentityImport import in this.RewardIdentities)
            {
                if (string.IsNullOrEmpty(import.RewardIdentifier))
                {
                    Logging.DebugLog(WellknownLoggingLevels.Error, WellknownLoggingCategories.DatabaseImportCompletion, $"{nameof(import.RewardIdentifier)} is empty for PickRewardImport id '{this.Id}'.");
                }

                rewardIdentities.Add(new Reward(import.RewardIdentifier, import.IdentityKind));
            }

            return new PickRewardPrototype(this.Id, this.ProtocolArgument, rewardIdentities);
        }
    }
}