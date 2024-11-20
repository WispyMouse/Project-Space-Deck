namespace SpaceDeck.Tokenization.Processing
{
    using SpaceDeck.Tokenization.Minimum;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Static helper class for turning <see cref="TokenText"/> into <see cref="ParsedTokenSet"/>.
    /// </summary>
    public static class LinkedTokenMaker
    {
        public static bool TryGetLinkedTokenSet(ParsedTokenSet parsedTokenSet, out LinkedTokenSet linkedTokenSet)
        {
            List<LinkedToken> linkedTokens = new List<LinkedToken>();

            foreach (ParsedToken parsedToken in parsedTokenSet.Tokens)
            {
                if (!parsedToken.CommandToExecute.TryGetLinkedToken(parsedToken, out LinkedToken linkedToken))
                {
                    // TODO log failure
                    linkedTokenSet = default(LinkedTokenSet);
                    return false;
                }

                linkedTokens.Add(linkedToken);
            }

            linkedTokenSet = new LinkedTokenSet(linkedTokens);
            return true;
        }
    }
}