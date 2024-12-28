namespace SpaceDeck.Models.Instances
{
    using System.Collections;
    using System.Collections.Generic;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Models.Prototypes;

    public class EncounterInstance : EncounterState
    {
        public readonly EncounterPrototype Prototype;

        public EncounterInstance(EncounterPrototype prototype) :base()
        {
            this.Prototype = prototype;
        }
    }
}