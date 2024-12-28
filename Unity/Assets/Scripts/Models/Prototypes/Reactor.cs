namespace SpaceDeck.Models.Prototypes
{
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.Utility.Minimum;
    using System.Collections;
    using System.Collections.Generic;

    public class Reactor
    {
        public readonly HashSet<LowercaseString> ReactionTriggers;
        public ParsedTokenList? ParsedTokens;
        public LinkedTokenList? LinkedTokens;

        public Reactor(IEnumerable<LowercaseString> reactionTriggers, ParsedTokenList parsedTokens)
        {
            this.ReactionTriggers = new HashSet<LowercaseString>(reactionTriggers);
            this.ParsedTokens = parsedTokens;
            this.LinkedTokens = null;
        }

        public Reactor(IEnumerable<LowercaseString> reactionTriggers, LinkedTokenList parsedTokens)
        {
            this.ReactionTriggers = new HashSet<LowercaseString>(reactionTriggers);
            this.ParsedTokens = null;
            this.LinkedTokens = parsedTokens;
        }
    }
}
