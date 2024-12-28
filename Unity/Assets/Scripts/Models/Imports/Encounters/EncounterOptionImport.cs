namespace SpaceDeck.Models.Imports
{
    using System;
    using System.Collections.Generic;

    [Serializable]

    public class EncounterOptionImport
    {
        public string RequirementScript;
        public string Dialogue;
        public List<EncounterOptionOutcomeImport> PossibleOutcomes;
    }
}