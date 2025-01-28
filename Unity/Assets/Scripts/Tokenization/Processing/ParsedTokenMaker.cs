namespace SpaceDeck.Tokenization.Processing
{
    using SpaceDeck.Tokenization.Minimum;
    using System.Collections;
    using System.Collections.Generic;
    using SpaceDeck.Utility.Minimum;
    using SpaceDeck.Utility.Logging;
    using SpaceDeck.Utility.Wellknown;

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
                        Logging.DebugLog(WellknownLoggingLevels.Error,
                            WellknownLoggingCategories.ParseTokenText,
                            $"Cannot find a scripting command using this identifier: '{statement.ScriptingCommandIdentifier}'");
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