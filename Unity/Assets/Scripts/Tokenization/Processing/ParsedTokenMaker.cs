namespace SpaceDeck.Tokenization.Processing
{
    using SpaceDeck.Tokenization.Minimum;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Static helper class for turning <see cref="TokenText"/> into <see cref="ParsedTokenList"/>.
    /// </summary>
    public static class ParsedTokenMaker
    {
        public static bool TryGetParsedTokensFromTokenText(TokenText baseText, out ParsedTokenList parsedSet)
        {
            List<ParsedToken> tokens = new List<ParsedToken>();

            foreach (TokenTextScope scope in baseText.Scopes)
            {
                foreach (TokenStatement statement in scope.Statements)
                {
                    if (!ScriptingCommandReference.TryGetScriptingCommandByIdentifier(statement.ScriptingCommandIdentifier, out ScriptingCommand command))
                    {
                        // TODO: This means something has been input incorrectly
                        // What's the best way to gracefully inform the user?
                        // Probably hooking up the logs soon
                        parsedSet = default(ParsedTokenList);
                        return false;
                    }

                    ParsedToken token = new ParsedToken(command, statement.Arguments);
                    tokens.Add(token);
                }
            }

            parsedSet = new ParsedTokenList(tokens);
            return true;
        }
    }
}