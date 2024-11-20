namespace SpaceDeck.Models.Prototypes
{
    using SpaceDeck.Tokenization.Minimum;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a card prototype in the game.
    /// 
    /// The Models.Instances library holds individual instances of the prototype.
    /// </summary>
    public class CardPrototype
    {
        public readonly ParsedTokenList ParsedTokens;
        public LinkedTokenList LinkedTokens { get; private set; }

        public CardPrototype(ParsedTokenList parsedTokens)
        {
            this.ParsedTokens = parsedTokens;
        }

        public void LinkTokens()
        {
            this.LinkedTokens = new LinkedTokenList();
        }
    }
}
