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

    public class CountElementEvaluatableValue : INumericEvaluatableValue
    {
        public Element ElementToCount;

        public CountElementEvaluatableValue(Element element)
        {
            this.ElementToCount = element;
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
            return this.TryEvaluate(context.ExecutedOnGameState, out value);
        }

        public bool TryEvaluate(IGameStateMutator mutator, out decimal value)
        {
            value = mutator.GetElement(this.ElementToCount);
            return true;
        }
    }

    public class CountElementEvaluatableParser : EvaluatableParser<CountElementEvaluatableValue, decimal>
    {
        public static readonly LowercaseString FunctionName = "COUNTELEMENT";

        public override bool TryParse(LowercaseString argument, out IEvaluatableValue<decimal> parsedValue)
        {
            if (!TryParseFunctionFromArgument(FunctionName, argument, out List<LowercaseString> argumentParts))
            {
                parsedValue = null;
                return false;
            }

            // There should be one argument, which is the currency id
            if (argumentParts.Count != 1)
            {
                parsedValue = null;
                return false;
            }

            Element matchedElement;
            if (!ElementDatabase.TryGetElement(argumentParts[0], out matchedElement))
            {
                parsedValue = null;
                return false;
            }

            parsedValue = new CountElementEvaluatableValue(matchedElement);
            return true;
        }
    }
}