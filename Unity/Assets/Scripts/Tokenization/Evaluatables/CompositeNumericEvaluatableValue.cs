namespace SpaceDeck.Tokenization.Evaluatables
{
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Utility.Minimum;
    using System.Collections.Generic;
    using SpaceDeck.Tokenization.Minimum.Questions;
    using SpaceDeck.Tokenization.Minimum.Context;
    using SpaceDeck.Utility.Logging;
    using SpaceDeck.Utility.Wellknown;

    public class CompositeNumericEvaluatableValue : INumericEvaluatableValue
    {
        public readonly List<CompositeRelationEntry> CompositeEntries = new List<CompositeRelationEntry>();

        public CompositeNumericEvaluatableValue(List<CompositeRelationEntry> entries)
        {
            this.CompositeEntries = entries;
        }

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
            decimal total = 0;

            foreach (CompositeRelationEntry entry in this.CompositeEntries)
            {
                if (!entry.InnerValue.TryEvaluate(context, out decimal thisValue))
                {
                    Logging.DebugLog(WellknownLoggingLevels.Error,
                        WellknownLoggingCategories.EvaluatableEvaluation,
                        $"Failed to evaluate this {nameof(CompositeNumericEvaluatableValue)} using a {nameof(ScriptingExecutionContext)}. Inner entry description: {entry.InnerValue.Describe()}");
                    value = 0;
                    return false;
                }

                switch (entry.RelationshipToPrevious)
                {
                    case CompositeRelationType.None:
                        total = thisValue;
                        break;
                    case CompositeRelationType.Addition:
                        total += thisValue;
                        break;
                    case CompositeRelationType.Subtraction:
                        total -= thisValue;
                        break;
                    case CompositeRelationType.Multiplication:
                        total *= thisValue;
                        break;
                    case CompositeRelationType.Division:
                        total /= thisValue;
                        break;
                    case CompositeRelationType.Random:
                        total = new System.Random().Next((int)total, (int)thisValue);
                        break;
                }
            }

            value = total;
            return true;
        }

        public bool TryEvaluate(IGameStateMutator mutator, out decimal value)
        {
            decimal total = 0;

            foreach (CompositeRelationEntry entry in this.CompositeEntries)
            {
                if (!entry.InnerValue.TryEvaluate(mutator, out decimal thisValue))
                {
                    Logging.DebugLog(WellknownLoggingLevels.Error,
                        WellknownLoggingCategories.EvaluatableEvaluation,
                        $"Failed to evaluate this {nameof(CompositeNumericEvaluatableValue)} using a {nameof(IGameStateMutator)}. Inner entry description: {entry.InnerValue.Describe()}");
                    value = 0;
                    return false;
                }

                switch (entry.RelationshipToPrevious)
                {
                    case CompositeRelationType.None:
                        total = thisValue;
                        break;
                    case CompositeRelationType.Addition:
                        total += thisValue;
                        break;
                    case CompositeRelationType.Subtraction:
                        total -= thisValue;
                        break;
                    case CompositeRelationType.Multiplication:
                        total *= thisValue;
                        break;
                    case CompositeRelationType.Division:
                        total /= thisValue;
                        break;
                    case CompositeRelationType.Random:
                        total = new System.Random().Next((int)total, (int)thisValue);
                        break;
                }
            }

            value = total;
            return true;
        }
    }

    public class CompositeNumericEvaluatableParser : EvaluatableParser<CompositeNumericEvaluatableValue, decimal>
    {
        public override bool TryParse(LowercaseString argument, out IEvaluatableValue<decimal> parsedValue)
        {
            parsedValue = new CompositeNumericEvaluatableValue(null);
            return true;
        }
    }

    public enum CompositeRelationType
    {
        None,
        Addition,
        Subtraction,
        Multiplication,
        Division,
        Random
    }

    public class CompositeRelationEntry
    {
        public readonly INumericEvaluatableValue InnerValue;
        public CompositeRelationType RelationshipToPrevious;

        public CompositeRelationEntry(INumericEvaluatableValue innerValue)
        {
            this.InnerValue = innerValue;
            this.RelationshipToPrevious = CompositeRelationType.None;
        }
    }
}