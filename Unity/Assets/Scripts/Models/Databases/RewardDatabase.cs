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
        public static Dictionary<LowercaseString, PickRewardPrototype> PickRewardData { get; private set; } = new Dictionary<LowercaseString, PickRewardPrototype>();


        public static PickReward GetReward(LowercaseString id)
        {
            PickRewardPrototype prototype = PickRewardData[id];

            List<Reward> rewards = new List<Reward>(prototype.Rewards);

            PickReward newReward = new PickReward(PickReward.PickRewardProtocol.ChooseX, prototype.PickNumber, prototype.Rewards);
            return newReward;
        }

        public static void AddReward(PickRewardImport toImport)
        {

        }

        public static void AddReward(PickRewardPrototype toImport)
        {
            PickRewardData.Add(toImport.Id, toImport);
        }

        
        public static void ClearDatabase()
        {
            PickRewardData.Clear();
        }
    }
}