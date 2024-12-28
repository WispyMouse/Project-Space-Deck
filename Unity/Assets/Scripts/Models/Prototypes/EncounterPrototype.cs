namespace SpaceDeck.Models.Prototypes
{
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.Utility.Minimum;
    using System.Collections;
    using System.Collections.Generic;

    public class EncounterPrototype
    {
        public readonly LowercaseString Id;

        public EncounterPrototype(LowercaseString id)
        {
            this.Id = id;
        }
    }
}