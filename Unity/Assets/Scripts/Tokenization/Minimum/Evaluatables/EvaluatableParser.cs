namespace SpaceDeck.Tokenization.Minimum.Evaluatables
{
    public abstract class EvaluatableParser
    {
        public abstract bool TryParse(LowercaseString argument, out IEvaluatableValue parsedValue);
    }

    public abstract class EvaluatableParser<T,V> : EvaluatableParser where T : IEvaluatableValue<V>
    {
        public override bool TryParse(LowercaseString argument, out IEvaluatableValue parsedValue)
        {
            return this.TryParse(argument, out parsedValue);
        }

        public abstract bool TryParse(LowercaseString argument, IEvaluatableValue<V> parsedValue);
    }
}