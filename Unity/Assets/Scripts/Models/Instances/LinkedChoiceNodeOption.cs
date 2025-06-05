namespace SpaceDeck.Models.Instances
{
    using System.Collections;
    using System.Collections.Generic;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.GameState.Deltas;
    using SpaceDeck.Models.Prototypes;
    using static SpaceDeck.GameState.Minimum.GameStateEventTrigger;
    using SpaceDeck.Utility.Logging;
    using SpaceDeck.Utility.Wellknown;

    public class LinkedChoiceNodeOption : ChoiceNodeOption
    {
        public readonly EncounterInstance WillEncounter;

        public LinkedChoiceNodeOption(ChoiceNodeOption option, EncounterInstance willEncounter) : base(option.WillEncounterId, option.Arguments)
        {
            this.WillEncounter = willEncounter;
        }
    }
}