namespace SpaceDeck.Tokenization.Evaluatables
{
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.GameState.Context;
    using System.Collections.Generic;
    using SpaceDeck.Tokenization.Minimum.Questions;
    using System;
    using SpaceDeck.Tokenization.Minimum.Context;

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

        public IReadOnlyList<ExecutionQuestion> GetQuestions(LinkedToken linkedToken) => Array.Empty<ExecutionQuestion>();

        public bool TryEvaluate(ScriptingExecutionContext context, out decimal value)
        {
            if(!this.ToNegate.TryEvaluate(context, out value))
            {
                return false;
            }

            value = value * -1M;
            return true;
        }
    }
}