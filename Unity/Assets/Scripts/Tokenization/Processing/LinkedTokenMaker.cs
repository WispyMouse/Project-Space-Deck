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
            List<LinkedToken> linkedTokens = new List<LinkedToken>();

            foreach (ParsedToken parsedToken in parsedTokenSet.Tokens)
            {
                if (!parsedToken.CommandToExecute.TryGetLinkedToken(parsedToken, out LinkedToken linkedToken))
                {
                    // TODO log failure
                    linkedTokenList = default(LinkedTokenList);
                    return false;
                }

                linkedTokens.Add(linkedToken);
            }

            linkedTokenList = new LinkedTokenList(linkedTokens);
            return true;
        }
    }
}