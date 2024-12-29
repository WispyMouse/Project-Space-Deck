namespace SpaceDeck.Models.Prototypes
{
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.Utility.Minimum;
    using System.Collections;
    using System.Collections.Generic;

    public class EncounterScript
    {
        public readonly LowercaseString Id;

        public EncounterScript(LowercaseString id)
        {
            this.Id = id;
        }
    }
}