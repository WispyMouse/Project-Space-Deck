namespace SpaceDeck.Tokenization.Evaluatables
{
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Utility.Minimum;
    using System.Collections.Generic;
    using SpaceDeck.Tokenization.Minimum.Context;

    public class IntegerNumericEvaluatableValue : IEvaluatableValue<int>, INumericEvaluatableValue
    {
        public readonly INumericEvaluatableValue NumericEvaluatableValue;

        public IntegerNumericEvaluatableValue(INumericEvaluatableValue decimalNumericValue)
        {
            this.NumericEvaluatableValue = decimalNumericValue;
        }

        public string Describe()
        {
            return this.NumericEvaluatableValue.Describe();
        }

        public IReadOnlyList<ExecutionQuestion> GetQuestions(LinkedToken linkedToken)
        {
            return this.NumericEvaluatableValue.GetQuestions(linkedToken);
        }

        public bool TryEvaluate(ScriptingExecutionContext context, out int value)
        {
            if (!this.NumericEvaluatableValue.TryEvaluate(context, out decimal decimalValue))
            {
                value = 0;
                return false;
            }
            value = (int)decimalValue;
            return true;
        }

        public bool TryEvaluate(IGameStateMutator mutator, out int value)
        {
            if (!this.NumericEvaluatableValue.TryEvaluate(mutator, out decimal decimalValue))
            {
                value = 0;
                return false;
            }
            value = (int)decimalValue;
            return true;
        }

        public bool TryEvaluate(ScriptingExecutionContext context, out decimal value)
        {
            return this.NumericEvaluatableValue.TryEvaluate(context, out value);
        }

        public bool TryEvaluate(IGameStateMutator mutator, out decimal value)
        {
            return this.NumericEvaluatableValue.TryEvaluate(mutator, out value);
        }
    }
}