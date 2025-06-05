namespace SpaceDeck.Models.Instances
{
    using System.Collections.Generic;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Models.Prototypes;
    using SpaceDeck.Utility.Minimum;

    public interface IEncounterProvider
    {
        EncounterInstance GetEncounter(LowercaseString encounterId, IEnumerable<string> arguments);
    }
}