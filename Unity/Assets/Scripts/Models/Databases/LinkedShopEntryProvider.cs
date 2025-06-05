namespace SpaceDeck.Models.Databases
{
    using System.Collections.Generic;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Models.Instances;
    using SpaceDeck.Utility.Logging;
    using SpaceDeck.Utility.Minimum;
    using SpaceDeck.Utility.Wellknown;

    public class LinkedShopEntryProvider : ILinkedShopEntryProvider
    {
        public static readonly LinkedShopEntryProvider Instance = new LinkedShopEntryProvider();

        private LinkedShopEntryProvider()
        {

        }

        public LinkedShopEntry GetShopEntry(LowercaseStringSet arguments, IEnumerable<IShopCost> costs)
        {
            // Try to get a card with these criteria
            LinkedCardInstance foundCard = CardDatabase.GetInstance(arguments);
            if (foundCard != null)
            {
                LinkedRewardInstance reward = new LinkedRewardInstance(gainedCards: new List<LinkedCardInstance>() { foundCard });
                return new LinkedShopEntry(costs, reward);
            }

            Logging.DebugLog(WellknownLoggingLevels.Error, WellknownLoggingCategories.ProviderEvaluation, $"Could not find an appropriate shop entry for '{arguments}'.");
            return null;
        }
    }
}