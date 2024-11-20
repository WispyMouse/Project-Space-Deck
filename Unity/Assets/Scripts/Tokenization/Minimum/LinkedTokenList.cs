namespace SpaceDeck.Tokenization.Minimum
{
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Describes a stack of LinkedTokens.
    /// 
    /// TODO: Cache "First" such that it isn't requeried each time.
    /// </summary>
    public struct LinkedTokenList
    {
        public List<LinkedTokenScope> Scopes;
        public LinkedToken First
        {
            get
            {
                if (this.Scopes == null || this.Scopes.Count == 0 || this.Scopes[0].Tokens == null || this.Scopes[0].Tokens.Count == 0)
                {
                    return null;
                }

                return this.Scopes[0].Tokens[0];
            }
        }

        public LinkedTokenList(List<LinkedTokenScope> scopes)
        {
            this.Scopes = scopes;
        }
    }
}