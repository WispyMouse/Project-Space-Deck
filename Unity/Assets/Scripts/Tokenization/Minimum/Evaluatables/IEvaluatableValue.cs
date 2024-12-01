using SpaceDeck.GameState.Minimum;

namespace SpaceDeck.Tokenization.Minimum.Evaluatables
{
    public interface IEvaluatableValue
    {

    }

    public interface IEvaluatableValue<T> : IEvaluatableValue
    {
        public bool TryEvaluate(ExecutionContext context, out T value);
    }
}