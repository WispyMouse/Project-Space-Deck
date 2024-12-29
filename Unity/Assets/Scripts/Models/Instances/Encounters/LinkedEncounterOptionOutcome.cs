namespace SpaceDeck.GameState.Minimum
{
    using SpaceDeck.Utility.Minimum;
    using System.Collections;
    using System.Collections.Generic;

    public class LinkedEncounterOptionOutcome : EncounterOptionOutcome
    {
        public LinkedEncounterOptionOutcome(LowercaseString requirementScript, string dialogue, LowercaseString executionScript) : base(requirementScript, dialogue, executionScript)
        {
        }
    }
}
