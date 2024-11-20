namespace SpaceDeck.Tokenization.Minimum
{
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Describes a scope of <see cref="ParsedToken"/>.
    /// 
    /// A scope is an associated group of <see cref="ParsedToken"/>s.
    /// A chain of scopes is <see cref="ParsedTokenList"/>.
    /// </summary>
    public class ParsedTokenScope
    {
        public readonly List<ParsedToken> Tokens;

        public ParsedToken NextStatementAfterScope;

        public ParsedTokenScope(List<ParsedToken> tokens)
        {
            this.Tokens = tokens;
        }
    }
}
