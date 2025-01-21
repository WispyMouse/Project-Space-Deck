namespace SpaceDeck.Tokenization.Functions
{
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Utility.Minimum;
    using System.Collections.Generic;
    using SpaceDeck.Tokenization.Minimum.Questions;
    using SpaceDeck.Tokenization.Minimum.Context;
    using SpaceDeck.Tokenization.Evaluatables;
    using SpaceDeck.Models.Databases;

    public class CountStacksEvaluatableValue : INumericEvaluatableValue
    {
        public readonly ChangeTargetEvaluatableValue StacksOnTarget;
        public readonly LowercaseString StatusEffectToApply;

        public CountStacksEvaluatableValue(ChangeTargetEvaluatableValue onTarget, LowercaseString statusEffect)
        {
            this.StacksOnTarget = onTarget;
            this.StatusEffectToApply = statusEffect;
        }

        public string Describe()
        {
            throw new System.NotImplementedException();
        }

        public IReadOnlyList<ExecutionQuestion> GetQuestions(LinkedToken linkedToken)
        {
            return this.StacksOnTarget.GetQuestions(linkedToken);
        }

        public bool TryEvaluate(ScriptingExecutionContext context, out decimal value)
        {
            value = 0;

            if (!this.StacksOnTarget.TryEvaluate(context, out IChangeTarget target))
            {
                return false;
            }

            foreach (Entity entity in target.GetRepresentedEntities(context.ExecutedOnGameState))
            {
                value += context.ExecutedOnGameState.GetStacks(entity, this.StatusEffectToApply);
            }

            return true;
        }

        public bool TryEvaluate(IGameStateMutator mutator, out decimal value)
        {
            value = 0;

            if (!this.StacksOnTarget.TryEvaluate(mutator, out IChangeTarget target))
            {
                return false;
            }

            foreach (Entity entity in target.GetRepresentedEntities(mutator))
            {
                value += mutator.GetStacks(entity, this.StatusEffectToApply);
            }

            return true;
        }
    }

    public class CountStacksEvaluatableParser : EvaluatableParser<CountStacksEvaluatableValue, decimal>
    {
        public static readonly LowercaseString FunctionName = "COUNTSTACKS";

        public override bool TryParse(LowercaseString argument, out IEvaluatableValue<decimal> parsedValue)
        {
            parsedValue = null;

            if (!TryParseFunctionFromArgument(FunctionName, argument, out List<LowercaseString> argumentParts))
            {
                return false;
            }

            // There should be a target, then an id
            if (argumentParts.Count != 2)
            {
                return false;
            }

            // First entry is a target
            if (!EvaluatablesReference.TryGetChangeTargetEvaluatableValue(argumentParts[0], out ChangeTargetEvaluatableValue changeTarget))
            {
                return false;
            }

            // Second entry is a status effect id
            if (!StatusEffectDatabase.HasEntry(argumentParts[1]))
            {
                return false;
            }

            parsedValue = new CountStacksEvaluatableValue(changeTarget, argumentParts[1]);
            return true;
        }
    }
}