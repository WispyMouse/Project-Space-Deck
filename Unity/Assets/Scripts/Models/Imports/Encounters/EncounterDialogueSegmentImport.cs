namespace SpaceDeck.Models.Imports
{
    using SpaceDeck.Models.Prototypes;
    using System;
    using System.Collections.Generic;

    [Serializable]

    public class EncounterDialogueSegmentImport
    {
        public string RequirementScript;
        public string Dialogue;

        public EncounterDialogueSegment GetSegment()
        {
            return new EncounterDialogueSegment(this.RequirementScript, this.Dialogue);
        }
    }
}