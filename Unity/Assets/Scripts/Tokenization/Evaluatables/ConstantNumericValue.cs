namespace SpaceDeck.Tokenization.Evaluatables
{
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.GameState.Minimum;

    /// <summary>
    /// Describes a <see cref="ConstantEvaluatableValue{T}"/> that represents
    /// a <see cref="decimal"/>.
    /// 
    /// This can be used for math operations in a simple way that requires no
    /// context to evaluate.
    /// </summary>
    public class ConstantNumericValue : ConstantEvaluatableValue<decimal>, INumericEvaluatableValue
    {
        public ConstantNumericValue(int constant) : base(constant)
        {
        }

        public ConstantNumericValue(decimal constant) : base(constant)
        {
        }
    }

    public class ConstantNumericEvaluatableParser : EvaluatableParser<ConstantEvaluatableValue<decimal>, decimal>
    {
        public override bool TryParse(LowercaseString argument, out IEvaluatableValue<decimal> parsedValue)
        {
            if (decimal.TryParse(argument.ToString(), out decimal parsedDecimal))
            {
                parsedValue = new ConstantNumericValue(parsedDecimal);
                return true;
            }
            parsedValue = null;
            return false;
        }
    }
}