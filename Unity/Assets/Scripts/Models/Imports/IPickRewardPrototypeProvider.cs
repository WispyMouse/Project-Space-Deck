namespace SpaceDeck.Models.Imports
{
    using SpaceDeck.Models.Prototypes;
    using System.Collections.Generic;

    public interface IPickRewardPrototypeProvider
    {
        IEnumerable<PickRewardPrototype> GetPrototypes(IEnumerable<PickRewardImport> rewards);
    }
}