namespace SpaceDeck.Tokenization.Processing
{
    using SpaceDeck.Tokenization.Minimum;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Static helper class for turning <see cref="TokenText"/> into <see cref="ParsedTokenList"/>.
    /// </summary>
    public static class LinkedTokenMaker
    {
        public static bool TryGetLinkedTokenList(ParsedTokenList parsedTokenSet, out LinkedTokenList linkedTokenList)
        {
            List<LinkedTokenScope> linkedScopes = new List<LinkedTokenScope>();
            LinkedTokenScope previousScope = null;
            LinkedToken previousToken = null;

            foreach (ParsedTokenScope scope in parsedTokenSet.Scopes)
            {
                List<LinkedToken> linkedTokens = new List<LinkedToken>();
                LinkedTokenScope linkedScope = new LinkedTokenScope(linkedTokens);
                linkedScopes.Add(linkedScope);

                foreach (ParsedToken parsedToken in scope.Tokens)
                {
                    if (!parsedToken.CommandToExecute.TryGetLinkedToken(parsedToken, out LinkedToken linkedToken))
                    {
                        // TODO log failure
                        linkedTokenList = default(LinkedTokenList);
                        return false;
                    }

                    linkedTokens.Add(linkedToken);
                    linkedToken.LinkedScope = linkedScope;

                    // If this is the first statement after a scope has closed, then assign
                    // that scope's "Next" to this Statement's ParsedToken
                    if (previousScope != null && previousScope != linkedScope)
                    {
                        previousScope.NextStatementAfterScope = linkedToken;
                    }

                    if (previousToken != null)
                    {
                        previousToken.NextToken = parsedToken;
                        previousToken.NextLinkedToken = linkedToken;
                    }

                    previousToken = linkedToken;
                }

                previousScope = linkedScope;
            }

            linkedTokenList = new LinkedTokenList(linkedScopes);
            return true;
        }

        public static LinkedTokenList CreateTokenListFromLinkedTokens(params LinkedToken[] linkedTokens)
        {
            List<LinkedToken> tokens = new List<LinkedToken>();
            LinkedTokenScope onlyScope = new LinkedTokenScope(tokens);

            foreach (LinkedToken token in linkedTokens)
            {
                tokens.Add(token);
            }

            return new LinkedTokenList(new List<LinkedTokenScope>() { onlyScope });
        }
    }
}