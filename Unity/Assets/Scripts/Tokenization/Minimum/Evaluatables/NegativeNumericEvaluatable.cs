namespace SpaceDeck.Tokenization.Minimum.Evaluatables
{
    /// <summary>
    /// Describes a <see cref="ConstantEvaluatableValue{T}"/> that represents
    /// a <see cref="decimal"/>.
    /// 
    /// This can be used for math operations in a simple way that requires no
    /// context to evaluate.
    /// </summary>
    public class NegativeNumericEvaluatableValue : INumericEvaluatableValue
    {
        public readonly INumericEvaluatableValue ToNegate;

        public NegativeNumericEvaluatableValue(INumericEvaluatableValue toNegate)
        {
            this.ToNegate = toNegate;
        }

        public bool TryEvaluate(out decimal value)
        {
            if(!this.ToNegate.TryEvaluate(out value))
            {
                return false;
            }

            value = value * -1M;
            return true;
        }
    }
}