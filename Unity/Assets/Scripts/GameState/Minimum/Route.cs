namespace SpaceDeck.GameState.Minimum
{
    using SpaceDeck.GameState.Minimum;
    using System.Collections;
    using System.Collections.Generic;
    using SpaceDeck.Utility.Minimum;

    public class Route : IHaveQualities
    {
        public readonly string Name;
        public readonly List<ChoiceNode> Choices = new List<ChoiceNode>();
        public QualitiesHolder Qualities => this._Qualities;
        protected readonly QualitiesHolder _Qualities = new QualitiesHolder();
        public readonly List<LowercaseString> StartingCards = new List<LowercaseString>();

        public Route(string name, List<ChoiceNode> choices, QualitiesHolder qualities, List<LowercaseString> startingCards)
        {
            this.Name = name;
            this.Choices = choices;
            this._Qualities = qualities;
            this.StartingCards = startingCards;
        }

    }
}