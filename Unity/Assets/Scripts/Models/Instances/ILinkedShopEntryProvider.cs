namespace SpaceDeck.Models.Instances
{
    using System.Collections.Generic;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Utility.Minimum;

    public interface ILinkedShopEntryProvider
    {
        LinkedShopEntry GetShopEntry(LowercaseStringSet arguments, IEnumerable<IShopCost> costs);
    }
}