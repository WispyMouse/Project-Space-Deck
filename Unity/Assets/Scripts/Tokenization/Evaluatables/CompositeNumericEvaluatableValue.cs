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
    using System.Text;

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
            List<ExecutionQuestion> questions = new List<ExecutionQuestion>();

            foreach (CompositeRelationEntry relation in this.CompositeEntries)
            {
                IReadOnlyList<ExecutionQuestion> relationQuestions = relation.InnerValue.GetQuestions(linkedToken);
                if (relationQuestions != null && relationQuestions.Count > 0)
                {
                    questions.AddRange(relationQuestions);
                }
            }

            return questions;
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
                    default:
                        Logging.DebugLog(WellknownLoggingLevels.Error, WellknownLoggingCategories.EvaluatableEvaluation, $"Unhandled relationship type: '{entry.RelationshipToPrevious}'");
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
            // Theoretically, a CompositeNumericEvaluatableValue could be any kind of thing combined with any other kind of thing
            // So to try to head off accidentally tesselating, the argument must contain at least one known operator
            if (!argument.ContainsAnyOf('+', '-', '/', '*', '~'))
            {
                parsedValue = null;
                return false;
            }

            List<CompositeRelationEntry> entries = new List<CompositeRelationEntry>();

            StringBuilder currentArgument = new StringBuilder();
            CompositeRelationType previousRelation = CompositeRelationType.None;

            for (int index = 0; index < argument.Value.Length; index++)
            {
                CompositeRelationType nextRelation;

                // Read through each character. If the character is any of these
                // special operator characters, then set the relation type and 
                // continue execution of for loop.
                // If it is any other character, then push it to the current argument stack.
                // If this is an operation, then try to evaluate the previous argument stack.
                // Add it to the entries list, and continue evaluation.
                // This will also be done at the end of the loop.
                switch (argument.Value[index])
                {
                    case '+':
                        nextRelation = CompositeRelationType.Addition;
                        break;
                    case '-':
                        nextRelation = CompositeRelationType.Subtraction;
                        break;
                    case '*':
                        nextRelation = CompositeRelationType.Multiplication;
                        break;
                    case '/':
                        nextRelation = CompositeRelationType.Division;
                        break;
                    case '~':
                        nextRelation = CompositeRelationType.Random;
                        break;
                    default:
                        currentArgument.Append(argument.Value[index]);
                        // Continue the for loop; no more code in this index loop will execute
                        continue;
                }

                // If the code executes here, the previous thing was an operator
                // Try to evaluate the stack we have now
                LowercaseString argumentString = currentArgument.ToString();
                currentArgument.Clear();
                if (!TryPushPreviousEvaluatable(argumentString, previousRelation, entries))
                {
                    parsedValue = null;
                    return false;
                }
                previousRelation = nextRelation;
            }

            // Composites should certainly end with a evaluatable argument
            if (!TryPushPreviousEvaluatable(currentArgument.ToString(), previousRelation, entries))
            {
                parsedValue = null;
                return false;
            }

            parsedValue = new CompositeNumericEvaluatableValue(entries);
            return true;
        }

        bool TryPushPreviousEvaluatable(string evaluationText, CompositeRelationType previousRelation, List<CompositeRelationEntry> compositeEntries)
        {
            if (!EvaluatablesReference.TryGetNumericEvaluatableValue(evaluationText, out INumericEvaluatableValue evaluatable))
            {
                Logging.DebugLog(WellknownLoggingLevels.Error,
                    WellknownLoggingCategories.GetLinkedScriptingToken,
                    $"Failed to parse numeric evaluatalbe argument out of sub-argument. Sub-Argument: '{evaluationText}'");
                return false;
            }

            compositeEntries.Add(new CompositeRelationEntry(evaluatable, previousRelation));
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
        public readonly CompositeRelationType RelationshipToPrevious;

        public CompositeRelationEntry(INumericEvaluatableValue innerValue, CompositeRelationType relationship)
        {
            this.InnerValue = innerValue;
            this.RelationshipToPrevious = relationship;
        }
    }
}