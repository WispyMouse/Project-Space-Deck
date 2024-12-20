namespace SpaceDeck.Tokenization.Processing
{
    using SpaceDeck.Tokenization.Minimum;
    using System.Collections;
    using System.Collections.Generic;
    using SpaceDeck.Utility.Minimum;

    /// <summary>
    /// Static helper class for turning <see cref="TokenText"/> into <see cref="ParsedTokenList"/>.
    /// </summary>
    public static class ParsedTokenMaker
    {
        public static bool TryGetParsedTokensFromTokenText(TokenText baseText, out ParsedTokenList parsedSet)
        {
            List<ParsedTokenScope> parsedScopes = new List<ParsedTokenScope>();
            ParsedTokenScope previousScope = null;
            ParsedToken previousToken = null;

            foreach (TokenTextScope scope in baseText.Scopes)
            {
                List<ParsedToken> parsedTokens = new List<ParsedToken>();
                ParsedTokenScope parsedScope = new ParsedTokenScope(parsedTokens);
                parsedScopes.Add(parsedScope);

                foreach (TokenStatement statement in scope.Statements)
                {
                    if (!ScriptingCommandReference.TryGetScriptingCommandByIdentifier(statement.ScriptingCommandIdentifier, out ScriptingCommand scriptingCommand))
                    {
                        // TODO Log failure
                        parsedSet = default(ParsedTokenList);
                        return false;
                    }

                    // TODO Validate that the provided Arguments are potentially valid for the selected command

                    ParsedToken parsedToken = new ParsedToken(scriptingCommand, statement.Arguments);
                    parsedTokens.Add(parsedToken);
                    parsedToken.Scope = parsedScope;

                    // If this is the first statement after a scope has closed, then assign
                    // that scope's "Next" to this Statement's ParsedToken
                    if (previousScope != null && previousScope != parsedScope)
                    {
                        previousScope.NextStatementAfterScope = parsedToken;
                    }

                    if (previousToken != null)
                    {
                        previousToken.NextToken = parsedToken;
                    }

                    previousToken = parsedToken;
                }

                previousScope = parsedScope;
            }

            parsedSet = new ParsedTokenList(parsedScopes);
            return true;
        }
    }
}