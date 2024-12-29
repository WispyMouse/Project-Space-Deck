namespace SpaceDeck.Models.Databases
{
    using System.Collections;
    using System.Collections.Generic;
    using SpaceDeck.Models.Prototypes;
    using SpaceDeck.Models.Instances;
    using SpaceDeck.Utility.Minimum;
    using SpaceDeck.Tokenization.Processing;
    using SpaceDeck.Tokenization.Minimum;

    public static class CardDatabase
    {
        private readonly static Dictionary<LowercaseString, CardPrototype> Prototypes = new Dictionary<LowercaseString, CardPrototype>();

        public static void RegisterCardPrototype(CardPrototype prototype)
        {
            Prototypes.Add(prototype.Id, prototype);
        }

        public static LinkedCardInstance GetInstance(LowercaseString id)
        {
            return new LinkedCardInstance(Prototypes[id]);
        }

        public static void ClearDatabase()
        {
            Prototypes.Clear();
        }

        public static void LinkTokens()
        {
            foreach (CardPrototype curPrototype in Prototypes.Values)
            {
                if (!curPrototype.LinkedTokens.HasValue && curPrototype.ParsedTokens.HasValue)
                {
                    if (LinkedTokenMaker.TryGetLinkedTokenList(curPrototype.ParsedTokens.Value, out LinkedTokenList linkedTokens))
                    {
                        curPrototype.LinkedTokens = linkedTokens;
                    }
                }
            }
        }
    }
}