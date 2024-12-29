namespace SpaceDeck.Tokenization.Evaluatables
{
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Utility.Minimum;
    using System.Collections.Generic;
    using SpaceDeck.Tokenization.Minimum.Questions;
    using SpaceDeck.Tokenization.Minimum.Context;

    public class ConstantCardsEvaluatableValue : CardsEvaluatableValue, IConstantEvaluatableValue<IReadOnlyList<CardInstance>>
    {
        public IReadOnlyList<CardInstance> Constant => this._Constant;
        private readonly IReadOnlyList<CardInstance> _Constant;

        public ConstantCardsEvaluatableValue(IReadOnlyList<CardInstance> card)
        {
            this._Constant = card;
        }
    }
}