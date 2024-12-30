namespace SpaceDeck.GameState.Minimum
{
    using SpaceDeck.GameState.Minimum;
    using System.Collections;
    using System.Collections.Generic;
    using SpaceDeck.Utility.Minimum;

    public class Route
    {
        public readonly string Name;
        public readonly List<ChoiceNode> Choices = new List<ChoiceNode>();

        public Route(string name, List<ChoiceNode> choices)
        {
            this.Name = name;
            this.Choices = choices;
        }
    }
}