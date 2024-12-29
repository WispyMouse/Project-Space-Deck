namespace SpaceDeck.Models.Imports
{
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Models.Prototypes;
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class EncounterOptionOutcomeImport
    {
        public string Criteria;
        public string Dialogue;
        public string Effect;

        public EncounterOptionOutcome GetOutcome()
        {
            return new EncounterOptionOutcome(this.Criteria, this.Dialogue, this.Effect);
        }
    }
}