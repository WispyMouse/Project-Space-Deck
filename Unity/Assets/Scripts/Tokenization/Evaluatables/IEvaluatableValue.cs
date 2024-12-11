namespace SpaceDeck.Tokenization.Evaluatables
{
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.GameState.Context;
    using SpaceDeck.Tokenization.Minimum.Questions;
    using System.Collections.Generic;
    using SpaceDeck.Tokenization.Minimum.Context;

    public interface IEvaluatableValue
    {
        public IReadOnlyList<ExecutionQuestion> GetQuestions(LinkedToken linkedToken);
    }

    public interface IEvaluatableValue<T> : IEvaluatableValue
    {
        public bool TryEvaluate(ScriptingExecutionContext context, out T value);
    }
}