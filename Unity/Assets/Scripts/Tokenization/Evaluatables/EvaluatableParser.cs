namespace SpaceDeck.Tokenization.Evaluatables
{
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Utility.Minimum;

    public abstract class EvaluatableParser
    {
        public abstract bool TryParse(LowercaseString argument, out IEvaluatableValue parsedValue);
    }

    public abstract class EvaluatableParser<T,V> : EvaluatableParser where T : IEvaluatableValue<V>
    {
        public override bool TryParse(LowercaseString argument, out IEvaluatableValue parsedValue)
        {
            if (this.TryParse(argument, out IEvaluatableValue<V> boxedEvaluatedParsedValue))
            {
                parsedValue = boxedEvaluatedParsedValue;
                return true;
            }

            parsedValue = null;
            return false;
        }

        public abstract bool TryParse(LowercaseString argument, out IEvaluatableValue<V> parsedValue);
    }
}