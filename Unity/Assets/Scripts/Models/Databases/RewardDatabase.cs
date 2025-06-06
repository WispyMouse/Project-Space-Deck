namespace SpaceDeck.Models.Databases
{
    using System;
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


        public static PickRewardPrototype GetPickRewardPrototype(LowercaseString id)
        {
            return PickRewardData[id];
        }

        public static PickReward GetPickReward(LowercaseString id)
        {
            PickRewardPrototype prototype = PickRewardData[id];
            return GetPickReward(prototype);
        }

        public static PickReward GetPickReward(LowercaseStringSet argument)
        {
            if (argument.OnlyValue.HasValue)
            {
                return GetPickReward(argument.OnlyValue.Value);
            }

            // TODO LOG
            return null;
        }

        public static PickReward GetPickReward(PickRewardPrototype prototype)
        {
            List<RewardPrototype> rewards = new List<RewardPrototype>(prototype.Rewards);

            PickReward newReward = new PickReward(PickReward.PickRewardProtocol.ChooseX, prototype.PickNumber, prototype.Rewards);
            return newReward;
        }

        public static void AddPickReward(PickRewardImport toImport)
        {
            AddPickReward(toImport.GetPrototype());
        }

        public static void AddPickReward(PickRewardPrototype toImport)
        {
            PickRewardData.Add(toImport.Id, toImport);
        }

        
        public static void ClearDatabase()
        {
            PickRewardData.Clear();
        }

        public static LinkedRewardInstance GetReward(LowercaseStringSet criteria)
        {
            List<LinkedCardInstance> gainedCards = new List<LinkedCardInstance>();
            Dictionary<Currency, int> gainedCurrency = new Dictionary<Currency, int>();

            if (criteria.Strings.Contains("[card]"))
            {
                gainedCards.Add(CardDatabase.GetInstance(criteria));
            }
            else
            {
                // oops todo
            }

            return new LinkedRewardInstance(gainedCards, gainedCurrency);
        }

        public static LinkedRewardInstance GetReward(RewardPrototype rewardPrototype)
        {
            List<LinkedCardInstance> gainedCards = new List<LinkedCardInstance>();
            Dictionary<Currency, int> gainedCurrency = new Dictionary<Currency, int>();

            switch (rewardPrototype.IdentityKind)
            {
                case RewardPrototype.RewardIdentityKind.Card:
                    gainedCards.Add(CardDatabase.GetInstance(rewardPrototype.Id));
                    break;
                case RewardPrototype.RewardIdentityKind.Artifact:
                    // TODO
                    break;
                case RewardPrototype.RewardIdentityKind.Currency:
                    // TODO
                    break;
            }

            return new LinkedRewardInstance(gainedCards, gainedCurrency);
        }

        public static IEnumerable<IShopCost> GetCosts(LinkedRewardInstance basedOn)
        {
            // TODO!
            return Array.Empty<IShopCost>();
        }
    }
}