namespace SpaceDeck.Tokenization.Evaluatables
{
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.GameState.Context;
    using System.Collections.Generic;
    using SpaceDeck.Tokenization.Minimum.Questions;
    using System;
    using SpaceDeck.Tokenization.Minimum.Context;

    public interface IConstantEvaluatableValue<T> : IEvaluatableValue<T>
    {
        public T Constant { get; }
    }
}