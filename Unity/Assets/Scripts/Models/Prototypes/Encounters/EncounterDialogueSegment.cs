namespace SpaceDeck.Models.Prototypes
{
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.Utility.Minimum;
    using System.Collections;
    using System.Collections.Generic;

    public class EncounterDialogueSegment
    {
        public readonly LowercaseString RequirementScript;
        public readonly string Dialogue;

        public EncounterDialogueSegment(LowercaseString requirementScript, string dialogue)
        {
            this.RequirementScript = requirementScript;
            this.Dialogue = dialogue;
        }
    }
}
