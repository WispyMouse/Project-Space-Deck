namespace SpaceDeck.Tokenization.Evaluatables
{
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.GameState.Context;
    using SpaceDeck.Tokenization.Minimum.Questions;
    using System.Collections.Generic;

    public interface IEvaluatableValue
    {
        public IReadOnlyList<ExecutionQuestion> GetQuestions(LinkedToken linkedToken);
    }

    public interface IEvaluatableValue<T> : IEvaluatableValue
    {
        public bool TryEvaluate(ExecutionAnswerSet answers, out T value);
    }
}