namespace SpaceDeck.Models.Imports
{
    using SpaceDeck.Models.Prototypes;
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.Tokenization.Processing;
    using SpaceDeck.Utility.Minimum;
    using System.Collections;
    using System.Collections.Generic;

    public class ReactorImport
    {
        public List<string> TriggerOnEventIds;
        public string TokenText;

        public Reactor GetReactor()
        {
            HashSet<LowercaseString> triggerOnEventIds = new HashSet<LowercaseString>();
            foreach (string trigger in this.TriggerOnEventIds)
            {
                triggerOnEventIds.Add(trigger);
            }

            ParsedTokenList parsedTokens = default(ParsedTokenList);
            if (!(TokenTextMaker.TryGetTokenTextFromString(this.TokenText, out TokenText tokenText) && ParsedTokenMaker.TryGetParsedTokensFromTokenText(tokenText, out parsedTokens)))
            {
                // TODO LOG ERROR
            }

            return new Reactor(triggerOnEventIds, parsedTokens);
        }
    }
}