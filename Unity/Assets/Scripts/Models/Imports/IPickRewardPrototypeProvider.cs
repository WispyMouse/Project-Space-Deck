namespace SpaceDeck.Models.Imports
{
    using SpaceDeck.Models.Prototypes;
    using SpaceDeck.Utility.Minimum;
    using System.Collections.Generic;

    public interface IPickRewardPrototypeProvider
    {
        IEnumerable<PickRewardPrototype> GetPrototypes(IEnumerable<LowercaseString> rewards);
    }
}