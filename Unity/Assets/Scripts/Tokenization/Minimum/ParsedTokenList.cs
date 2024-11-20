namespace SpaceDeck.Tokenization.Minimum
{
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Describes a stack of <see cref="ParsedToken"/>s.
    /// </summary>
    public struct ParsedTokenList
    {
        public List<ParsedTokenScope> Scopes;

        public ParsedTokenList(List<ParsedTokenScope> scopes)
        {
            this.Scopes = scopes;
        }
    }
}