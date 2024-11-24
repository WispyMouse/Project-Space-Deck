namespace SpaceDeck.Tokenization.Minimum.Evaluatables
{
    public interface IEvaluatableValue
    {

    }

    public interface IEvaluatableValue<T> : IEvaluatableValue
    {
        public bool TryEvaluate(out T value);
    }
}