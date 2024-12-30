namespace SpaceDeck.Models.Databases
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Models.Imports;
    using SpaceDeck.Models.Instances;
    using SpaceDeck.Models.Prototypes;
    using SpaceDeck.Utility.Minimum;


    public class RewardDatabase
    {
        public static Dictionary<LowercaseString, RewardPrototype> RewardData { get; private set; } = new Dictionary<LowercaseString, RewardPrototype>();

        public static void AddReward(RewardImport toImport)
        {
            AddReward(toImport.GetReward());
        }

        public static void AddReward(RewardPrototype toImport)
        {
            RewardData.Add(toImport.Id, toImport);
        }

        public static RewardInstance GetInstance(LowercaseString id)
        {
            return new RewardInstance(RewardData[id]);
        }

        
        public static void ClearDatabase()
        {
            RewardData.Clear();
        }
    }
}