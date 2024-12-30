namespace SpaceDeck.GameState.Minimum
{
    using SpaceDeck.Utility.Minimum;
    using System.Collections.Generic;

    public class ChoiceNodeOption
    {
        public string NodeName => this.WillEncounter?.EncounterName;
        public EncounterState WillEncounter { get; private set; }
        public readonly LowercaseString WillEncounterId;

        public ChoiceNodeOption(LowercaseString encounter)
        {
            this.WillEncounterId = encounter;
        }

        public ChoiceNodeOption(EncounterState willEncounter) : this(willEncounter.EncounterId)
        {
            this.WillEncounter = willEncounter;
        }

        public void LinkEncounter(EncounterState willEncounter)
        {
            this.WillEncounter = willEncounter;
        }

        public string GetName()
        {
            return this.WillEncounter.EncounterName;
        }

        public string GetDescription()
        {
            return this.WillEncounter.EncounterDescription;
        }
    }
}