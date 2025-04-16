namespace SpaceDeck.Tokenization.Minimum
{
    using SpaceDeck.Utility.Minimum;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Describes a scope of <see cref="LinkedToken"/>.
    /// 
    /// A scope is an associated group of <see cref="LinkedToken"/>s.
    /// A chain of scopes is <see cref="LinkedTokenList"/>.
    /// </summary>
    public class LinkedTokenScope : IDescribable
    {
        public readonly List<LinkedToken> Tokens;

        public LinkedToken NextStatementAfterScope;

        public LinkedTokenScope(List<LinkedToken> tokens)
        {
            this.Tokens = tokens;
        }

        public string Describe()
        {
            StringBuilder description = new StringBuilder();
            string prependSpace = "";
            foreach (LinkedToken token in this.Tokens)
            {
                description.Append(prependSpace);
                description.Append(token.Describe());
                prependSpace = " ";
            }
            return description.ToString();
        }
    }
}
