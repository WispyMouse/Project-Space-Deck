namespace SpaceDeck.Models.Prototypes
{
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.Utility.Minimum;
    using System.Collections;
    using System.Collections.Generic;

    public class EncounterScript
    {
        public readonly LowercaseString Id;
        public readonly List<EncounterDialogueSegment> DialogueParts;
        public readonly List<EncounterOption> Options;

        public EncounterScript(LowercaseString id, IEnumerable<EncounterDialogueSegment> dialogueParts, IEnumerable<EncounterOption> options)
        {
            this.Id = id;
            this.Options = new List<EncounterOption>(options);
            this.DialogueParts = new List<EncounterDialogueSegment>(dialogueParts);
        }
    }
}