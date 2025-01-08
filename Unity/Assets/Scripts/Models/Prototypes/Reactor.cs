namespace SpaceDeck.Models.Prototypes
{
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.Utility.Minimum;
    using System.Collections;
    using System.Collections.Generic;
    using static SpaceDeck.GameState.Minimum.GameStateEventTrigger;

    public class Reactor
    {
        public readonly HashSet<LowercaseString> ReactionTriggers;
        public ParsedTokenList? ParsedTokens;
        public LinkedTokenList? LinkedTokens;
        public TriggerDirection Direction;

        public Reactor(IEnumerable<LowercaseString> reactionTriggers, ParsedTokenList parsedTokens, TriggerDirection direction)
        {
            this.ReactionTriggers = new HashSet<LowercaseString>(reactionTriggers);
            this.ParsedTokens = parsedTokens;
            this.LinkedTokens = null;
            this.Direction = direction;
        }

        public Reactor(IEnumerable<LowercaseString> reactionTriggers, LinkedTokenList linkedTokens, TriggerDirection direction)
        {
            this.ReactionTriggers = new HashSet<LowercaseString>(reactionTriggers);
            this.ParsedTokens = null;
            this.LinkedTokens = linkedTokens;
            this.Direction = direction;
        }
    }
}
