namespace SpaceDeck.Tokenization.Minimum
{
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Describes a stack of <see cref="ParsedToken"/>s.
    /// </summary>
    public struct ParsedTokenList
    {
        public List<ParsedToken> Tokens;

        public ParsedTokenList(List<ParsedToken> tokens)
        {
            this.Tokens = tokens;
        }
    }
}