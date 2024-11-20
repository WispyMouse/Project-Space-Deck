namespace SpaceDeck.Tokenization.Minimum
{
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Describes a stack of LinkedTokens.
    /// </summary>
    public struct LinkedTokenList
    {
        public List<LinkedToken> Tokens;

        public LinkedTokenList(List<LinkedToken> tokens)
        {
            this.Tokens = tokens;
        }
    }
}