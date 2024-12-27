namespace SpaceDeck.Models.Prototypes
{
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.Utility.Minimum;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a card prototype in the game.
    /// 
    /// The Models.Instances library holds individual instances of the prototype.
    /// </summary>
    public class CardPrototype
    {
        public readonly LowercaseString Id;
        public ParsedTokenList? ParsedTokens;
        public LinkedTokenList? LinkedTokens;

        public CardPrototype(LowercaseString id, ParsedTokenList parsedTokens)
        {
            this.Id = id;
            this.ParsedTokens = parsedTokens;
            this.LinkedTokens = null;
        }

        public CardPrototype(LowercaseString id, LinkedTokenList linkedTokens)
        {
            this.Id = id;
            this.ParsedTokens = null;
            this.LinkedTokens = linkedTokens;
        }
    }
}
