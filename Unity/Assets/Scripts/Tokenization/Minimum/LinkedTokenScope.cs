namespace SpaceDeck.Tokenization.Minimum
{
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Describes a scope of <see cref="LinkedToken"/>.
    /// 
    /// A scope is an associated group of <see cref="LinkedToken"/>s.
    /// A chain of scopes is <see cref="LinkedTokenList"/>.
    /// </summary>
    public class LinkedTokenScope
    {
        public readonly List<LinkedToken> Tokens;

        public LinkedToken NextStatementAfterScope;

        public LinkedTokenScope(List<LinkedToken> tokens)
        {
            this.Tokens = tokens;
        }
    }
}
