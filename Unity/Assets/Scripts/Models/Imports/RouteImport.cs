namespace SpaceDeck.Models.Imports
{
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Models.Prototypes;
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.Utility.Logging;
    using SpaceDeck.Utility.Minimum;
    using SpaceDeck.Utility.Wellknown;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    [System.Serializable]
    public class RouteImport : Importable
    {
        public string Name;
        public List<ChoiceNodeImport> Choices = new List<ChoiceNodeImport>();
        public List<StringQualityImport> StringQualities = new List<StringQualityImport>();
        public List<NumericQualityImport> NumberQualities = new List<NumericQualityImport>();
        public List<string> StartingCards = new List<string>();
        public List<NumericQualityImport> StartingCurrencies = new List<NumericQualityImport>();

        public Route GetRoute()
        {
            if (this.Choices == null || this.Choices.Count == 0)
            {
                Logging.DebugLog(WellknownLoggingLevels.Warning,
                    WellknownLoggingCategories.Route,
                    $"({this.Id}) {nameof(this.Choices)} is null or empty. This likely suggests the json is incorrect or missing content.");
            }

            List<ChoiceNode> nodes = new List<ChoiceNode>();
            foreach (ChoiceNodeImport import in ((IEnumerable<ChoiceNodeImport>)this.Choices ?? Array.Empty<ChoiceNodeImport>()))
            {
                nodes.Add(import.GetNode());
            }

            if (this.StartingCards == null || this.StartingCards.Count == 0)
            {
                Logging.DebugLog(WellknownLoggingLevels.Warning,
                    WellknownLoggingCategories.Route,
                    $"({this.Id}) {nameof(this.StartingCards)} is null or empty. This likely suggests the json is incorrect or missing content.");
            }

            List<LowercaseString> startingCards = new List<LowercaseString>();
            foreach (string startingCard in ((IEnumerable<string>)this.StartingCards ?? Array.Empty<string>()))
            {
                if (string.IsNullOrEmpty(startingCard))
                {
                    Logging.DebugLog(WellknownLoggingLevels.Error,
                        WellknownLoggingCategories.Route,
                        $"({this.Id}) Starting card id is null or empty. Each entry should be a string id.");
                    continue;
                }

                startingCards.Add(startingCard);
            }

            // Missing starting currency is more normal. Maybe we just don't have any starting currency in this one.

            Dictionary<LowercaseString, int> startingCurrencies = new Dictionary<LowercaseString, int>();
            foreach (NumericQualityImport startingCurrency in ((IEnumerable<NumericQualityImport>)this.StartingCurrencies ?? Array.Empty<NumericQualityImport>()))
            {
                if (string.IsNullOrEmpty(startingCurrency.Key))
                {
                    Logging.DebugLog(WellknownLoggingLevels.Error,
                        WellknownLoggingCategories.Route,
                        $"({this.Id}) Starting currency id is null or empty. Each entry should include a string id.");
                    continue;
                }

                startingCurrencies.Add(startingCurrency.Key, (int)startingCurrency.Value);
            }

            return new Route(this.Name, nodes, GetQualities(this.StringQualities, this.NumberQualities), startingCards, startingCurrencies);
        }
    }
}