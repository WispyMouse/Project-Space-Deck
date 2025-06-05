namespace SpaceDeck.GameState.Minimum
{
    using SpaceDeck.Utility.Minimum;
    using System;
    using System.Collections.Generic;

    public class ChoiceNodeOption
    {
        public readonly LowercaseString WillEncounterId;
        public readonly IReadOnlyList<string> Arguments;
        public bool WasSelected { get; set; } = false;

        public ChoiceNodeOption(LowercaseString encounter, IReadOnlyList<string> arguments = null)
        {
            this.WillEncounterId = encounter;
            this.Arguments = arguments ?? Array.Empty<string>();
        }
    }
}