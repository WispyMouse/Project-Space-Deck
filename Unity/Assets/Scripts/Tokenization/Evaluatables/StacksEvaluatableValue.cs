namespace SpaceDeck.Tokenization.Evaluatables
{
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Utility.Minimum;
    using System.Collections.Generic;
    using SpaceDeck.Tokenization.Minimum.Questions;
    using SpaceDeck.Tokenization.Minimum.Context;

    public class StacksEvaluatableValue : INumericEvaluatableValue
    {
        public string Describe()
        {
            throw new System.NotImplementedException();
        }

        public IReadOnlyList<ExecutionQuestion> GetQuestions(LinkedToken linkedToken)
        {
            throw new System.NotImplementedException();
        }

        public bool TryEvaluate(ScriptingExecutionContext context, out decimal value)
        {
            if (context.BasedOnAppliedStatusEffect == null)
            {
                value = 0;
                return false;
            }

            value = context.ExecutedOnGameState.GetStacks(context.User, context.BasedOnAppliedStatusEffect.Id);
            return true;
        }

        public bool TryEvaluate(IGameStateMutator mutator, out decimal value)
        {
            value = 0;
            return false;
        }
    }

    public class StacksEvaluatableParser : EvaluatableParser<StacksEvaluatableValue, decimal>
    {
        public override bool TryParse(LowercaseString argument, out IEvaluatableValue<decimal> parsedValue)
        {
            parsedValue = new StacksEvaluatableValue();
            return true;
        }
    }
}