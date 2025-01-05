namespace SpaceDeck.Tokenization.Minimum
{
    using SpaceDeck.Tokenization.Minimum.Questions;
    using SpaceDeck.Utility.Minimum;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Describes a stack of LinkedTokens.
    /// 
    /// TODO: Cache "First" such that it isn't requeried each time.
    /// </summary>
    public struct LinkedTokenList : IDescribable
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

        public LinkedTokenList(params LinkedToken[] unscopedTokens)
        {
            this.Scopes = new List<LinkedTokenScope>();
            LinkedTokenScope flatScope = new LinkedTokenScope(new List<LinkedToken>(unscopedTokens));
            this.Scopes.Add(flatScope);
        }

        public List<ExecutionQuestion> GetQuestions()
        {
            List<ExecutionQuestion> questions = new List<ExecutionQuestion>();

            LinkedToken nextToken = this.First;
            while (nextToken != null)
            {
                foreach (ExecutionQuestion question in nextToken.Questions)
                {
                    questions.Add(question);
                }

                nextToken = nextToken.NextLinkedToken;
            }

            return questions;
        }

        public string Describe()
        {
            return string.Empty;
        }
    }
}