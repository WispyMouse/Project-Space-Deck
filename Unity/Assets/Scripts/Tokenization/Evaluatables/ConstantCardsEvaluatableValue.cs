namespace SpaceDeck.Tokenization.Evaluatables
{
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Utility.Minimum;
    using System.Collections.Generic;
    using SpaceDeck.Tokenization.Minimum.Questions;
    using SpaceDeck.Tokenization.Minimum.Context;
    using SpaceDeck.Tokenization.Evaluatables.Questions;

    public class ConstantCardsEvaluatableValue : CardsEvaluatableValue, IConstantEvaluatableValue<IReadOnlyList<CardInstance>>
    {
        public IReadOnlyList<CardInstance> Constant => this._Constant;
        private readonly IReadOnlyList<CardInstance> _Constant;

        public ConstantCardsEvaluatableValue(IReadOnlyList<CardInstance> cards) : base(new SpecificCardsProvider(cards))
        {
            this._Constant = cards;
        }
    }
}