namespace SpaceDeck.Tokenization.Evaluatables
{
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.GameState.Context;
    using SpaceDeck.Tokenization.Minimum.Questions;
    using System.Collections.Generic;
    using SpaceDeck.Tokenization.Minimum.Context;
    using SpaceDeck.Utility.Minimum;

    public interface IEvaluatableValue : IDescribable
    {
        public IReadOnlyList<ExecutionQuestion> GetQuestions(LinkedToken linkedToken);
    }

    public interface IEvaluatableValue<T> : IEvaluatableValue
    {
        public bool TryEvaluate(ScriptingExecutionContext context, out T value);
        public bool TryEvaluate(IGameStateMutator mutator, out T value);
    }
}