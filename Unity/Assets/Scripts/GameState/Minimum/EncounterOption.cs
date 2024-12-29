namespace SpaceDeck.GameState.Minimum
{
    using SpaceDeck.Utility.Minimum;
    using System.Collections;
    using System.Collections.Generic;

    public class EncounterOption
    {
        public readonly LowercaseString RequirementScript;
        public readonly string Dialogue;
        public readonly List<EncounterOptionOutcome> PossibleOutcomes = new List<EncounterOptionOutcome>();

        public EncounterOption(LowercaseString requirementScript, string dialogue, List<EncounterOptionOutcome> outcomes)
        {
            this.RequirementScript = requirementScript;
            this.Dialogue = dialogue;
            this.PossibleOutcomes = outcomes;
        }
    }
}
