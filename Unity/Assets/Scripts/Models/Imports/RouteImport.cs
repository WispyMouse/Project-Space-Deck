namespace SpaceDeck.Models.Imports
{
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Models.Prototypes;
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.Utility.Minimum;
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

        public Route GetRoute()
        {
            List<ChoiceNode> nodes = new List<ChoiceNode>();
            foreach (ChoiceNodeImport import in this.Choices)
            {
                nodes.Add(import.GetNode());
            }
            List<LowercaseString> startingCards = new List<LowercaseString>();
            foreach (string startingCard in this.StartingCards)
            {
                startingCards.Add(startingCard);
            }

            return new Route(this.Name, nodes, GetQualities(this.StringQualities, this.NumberQualities), startingCards);
        }
    }
}