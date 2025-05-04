namespace SpaceDeck.Models.Databases
{
    using System.Collections.Generic;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Models.Imports;
    using SpaceDeck.Models.Instances;
    using SpaceDeck.Models.Prototypes;
    using SpaceDeck.Utility.Logging;
    using SpaceDeck.Utility.Minimum;
    using SpaceDeck.Utility.Wellknown;

    public class RewardDatabasePickRewardPrototypeProvider : IPickRewardPrototypeProvider
    {
        public static readonly RewardDatabasePickRewardPrototypeProvider Instance = new RewardDatabasePickRewardPrototypeProvider();

        public IEnumerable<PickRewardPrototype> GetPrototypes(IEnumerable<PickRewardImport> rewards)
        {
            List<PickRewardPrototype> prototypes = new List<PickRewardPrototype>();

            foreach (PickRewardImport imports in rewards)
            {
                prototypes.Add(imports.GetPrototype());
            }

            return prototypes;
        }
    }
}