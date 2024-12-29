namespace SpaceDeck.Models.Instances
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Models.Prototypes;
    using SpaceDeck.Utility.Minimum;

    public class LinkedEncounterOption : EncounterOption
    {
        public LinkedEncounterOption(LowercaseString requirementScript, string dialogue, List<EncounterOptionOutcome> outcomes) : base(requirementScript, dialogue, outcomes)
        {
        }
    }
}