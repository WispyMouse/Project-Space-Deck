namespace SpaceDeck.Models.Imports
{
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Models.Prototypes;
    using System;
    using System.Collections.Generic;

    [Serializable]

    public class EncounterScriptImport
    {
        public string Id;
        public List<EncounterDialogueSegmentImport> DialogueParts;
        public List<EncounterOptionImport> Options;

        public EncounterScript GetLinkedScript()
        {
            List<EncounterDialogueSegment> dialogueSegments = new List<EncounterDialogueSegment>();
            foreach (EncounterDialogueSegmentImport import in this.DialogueParts)
            {
                dialogueSegments.Add(import.GetSegment());
            }

            List<EncounterOption> options = new List<EncounterOption>();
            foreach (EncounterOptionImport import in this.Options)
            {
                List<EncounterOptionOutcome> outcomes = new List<EncounterOptionOutcome>();
                foreach (EncounterOptionOutcomeImport outcomeImport in import.PossibleOutcomes)
                {
                    outcomes.Add(outcomeImport.GetOutcome());
                }

                options.Add(new EncounterOption(import.RequirementScript, import.Dialogue, outcomes));
            }

            return new EncounterScript(this.Id,
                dialogueSegments,
                options);
        }
    }
}