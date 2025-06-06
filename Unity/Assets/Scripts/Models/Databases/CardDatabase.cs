namespace SpaceDeck.Models.Databases
{
    using System.Collections;
    using System.Collections.Generic;
    using SpaceDeck.Models.Prototypes;
    using SpaceDeck.Models.Instances;
    using SpaceDeck.Utility.Minimum;
    using SpaceDeck.Tokenization.Processing;
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Models.Imports;
    using SpaceDeck.Utility.Logging;
    using SpaceDeck.Utility.Wellknown;
    using System.Diagnostics;

    public static class CardDatabase
    {
        private readonly static Dictionary<LowercaseString, CardPrototype> Prototypes = new Dictionary<LowercaseString, CardPrototype>();

        public static void AddCardToDatabase(CardImport import)
        {
            // TODO: CardImport is Serializable, so it's not necessarily nullable
            // Can we do better at validating it is not empty?
            if (import == null)
            {
                Logging.DebugLog(WellknownLoggingLevels.Error,
                    WellknownLoggingCategories.CardImport,
                    $"Provided {nameof(CardImport)} is null.");
                return;
            }

            CardPrototype prototype = import.GetPrototype();

            if (prototype == null)
            {
                Logging.DebugLog(WellknownLoggingLevels.Error,
                    WellknownLoggingCategories.CardImport,
                    $"The {nameof(CardPrototype)} generated from {nameof(import.GetPrototype)} was null.");
                return;
            }

            RegisterCardPrototype(prototype);
        }

        public static void RegisterCardPrototype(CardPrototype prototype)
        {
            if (prototype == null)
            {
                Logging.DebugLog(WellknownLoggingLevels.Error,
                    WellknownLoggingCategories.CardImport,
                    $"Attempted to register a null card prototype.");
                return;
            }

            Prototypes.Add(prototype.Id, prototype);
        }

        public static LinkedCardInstance GetInstance(LowercaseString id, bool exactIdMatchOnly = false)
        {
            CardPrototype matchingPrototype = GetPrototype(id, exactIdMatchOnly);
            return new LinkedCardInstance(matchingPrototype, ElementDatabase.Provider);
        }

        public static LinkedCardInstance GetInstance(LowercaseStringSet criteria, bool exactIdMatchOnly = false)
        {
            CardPrototype matchingPrototype = GetPrototype(criteria, exactIdMatchOnly);

            if (matchingPrototype == null)
            {
                Logging.DebugLog(WellknownLoggingLevels.Error, WellknownLoggingCategories.CardDatabase, $"Could not find card instance that fits criteria '{criteria.ToString()}'");
            }

            return new LinkedCardInstance(matchingPrototype, ElementDatabase.Provider);
        }

        public static CardPrototype GetPrototype(LowercaseString id, bool exactIdMatchOnly = false)
        {
            LowercaseStringSet set = new LowercaseStringSet(id);
            return GetPrototype(set, exactIdMatchOnly);
        }

        public static CardPrototype GetPrototype(LowercaseStringSet criteria, bool exactIdMatchOnly = false)
        {
            if (criteria.OnlyValue.HasValue && Prototypes.TryGetValue(criteria.OnlyValue.Value, out CardPrototype exactMatch))
            {
                return exactMatch;
            }

            if (exactIdMatchOnly)
            {
                Logging.DebugLog(WellknownLoggingLevels.Error, WellknownLoggingCategories.CardDatabase, $"{nameof(GetPrototype)} called with {nameof(exactIdMatchOnly)}, but couldn't find an exact match to {criteria.ToString()}.");
                return null;
            }

            List<CardPrototype> matchingPrototypes = new List<CardPrototype>();
            foreach (CardPrototype prototype in Prototypes.Values)
            {
                bool matchbroken = false;

                foreach (LowercaseString tag in criteria.Strings)
                {
                    if (tag == "[card]" || CurrencyDatabase.IsCost(tag))
                    {
                        continue;
                    }

                    LowercaseString trimmedTag = tag.Value.TrimStart('[').TrimEnd(']');

                    // Is it in the tags?
                    if (prototype.Tags.Contains(trimmedTag))
                    {
                        continue;
                    }
                    // Is it the rarity?
                    else if (prototype.Qualities.GetStringQuality(WellknownQualities.Rarity) == trimmedTag)
                    {
                        continue;
                    }
                    // Is it the name?
                    else if (prototype.Qualities.GetStringQuality(WellknownQualities.Name) == trimmedTag)
                    {
                        continue;
                    }
                    // No match
                    else
                    {
                        matchbroken = true;
                        break;
                    }
                }

                if (!matchbroken)
                {
                    matchingPrototypes.Add(prototype);
                }
            }
            RandomDecider<CardPrototype> decider = new RandomDecider<CardPrototype>();
            return decider.ChooseRandomly(matchingPrototypes);
        }

        public static void ClearDatabase()
        {
            Prototypes.Clear();
        }

        public static void LinkTokens()
        {
            foreach (CardPrototype curPrototype in Prototypes.Values)
            {
                if (!(curPrototype.LinkedTokens.HasValue) && curPrototype.ParsedTokens.HasValue)
                {
                    if (!LinkedTokenMaker.TryGetLinkedTokenList(curPrototype.ParsedTokens.Value, out LinkedTokenList linkedTokens))
                    {
                        Logging.DebugLog(WellknownLoggingLevels.Error,
                            WellknownLoggingCategories.LinkingFailure,
                            $"Failure to get linked token list. Id '{curPrototype.Id}'");
                        continue;
                    }

                    curPrototype.LinkedTokens = linkedTokens;
                }
            }
        }

        public static IReadOnlyList<LinkedCardInstance> GetOneOfEveryCard()
        {
            List<LinkedCardInstance> cards = new List<LinkedCardInstance>();

            foreach (CardPrototype card in Prototypes.Values)
            {
                cards.Add(GetInstance(card.Id));
            }

            return cards;
        }
    }
}