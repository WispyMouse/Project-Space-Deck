namespace SpaceDeck.Models.Databases
{
    using System.Collections.Generic;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Models.Instances;
    using SpaceDeck.Models.Prototypes;
    using SpaceDeck.Utility.Logging;
    using SpaceDeck.Utility.Minimum;
    using SpaceDeck.Utility.Wellknown;

    public class EncounterDatabseEncounterProvider : IEncounterProvider
    {
        public static readonly EncounterDatabseEncounterProvider Instance = new EncounterDatabseEncounterProvider();

        private EncounterDatabseEncounterProvider()
        {

        }

        public EncounterInstance GetEncounter(LowercaseString encounterId, IEnumerable<string> arguments)
        {
            if (!EncounterDatabase.TryGetEncounterWithArguments(new DoNotRepeatRandomDecider<EncounterPrototype>(), encounterId, arguments, out EncounterInstance instance))
            {
                Logging.DebugLog(WellknownLoggingLevels.Error, WellknownLoggingCategories.GameState, $"Failed to get an encounter with the provided id '{encounterId}'.");
            }
            return instance;
        }
    }
}