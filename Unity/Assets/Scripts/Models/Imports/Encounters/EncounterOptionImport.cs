namespace SpaceDeck.Models.Imports
{
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Models.Prototypes;
    using System;
    using System.Collections.Generic;

    [Serializable]

    public class EncounterOptionImport
    {
        public string RequirementScript;
        public string Dialogue;
        public List<EncounterOptionOutcomeImport> PossibleOutcomes;

        public EncounterOption GetOption()
        {
            List<EncounterOptionOutcome> encounterOptionOutcomes = new List<EncounterOptionOutcome>();
            foreach (EncounterOptionOutcomeImport import in this.PossibleOutcomes)
            {
                encounterOptionOutcomes.Add(import.GetOutcome());
            }

            return new EncounterOption(this.RequirementScript, this.Dialogue, encounterOptionOutcomes);
        }
    }
}