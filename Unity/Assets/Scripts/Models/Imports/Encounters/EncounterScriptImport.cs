namespace SpaceDeck.Models.Imports
{
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
            return new EncounterScript(this.Id);
        }
    }
}