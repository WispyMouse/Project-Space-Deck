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

    public class CountCurrencyEvaluatableValue : INumericEvaluatableValue
    {
        public Currency Currency;

        public CountCurrencyEvaluatableValue(Currency currency)
        {
            this.Currency = currency;
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
            value = context.ExecutedOnGameState.GetCurrency(this.Currency);
            return true;
        }

        public bool TryEvaluate(IGameStateMutator mutator, out decimal value)
        {
            value = mutator.GetCurrency(this.Currency);
            return true;
        }
    }

    public class CountCurrencyEvaluatableParser : EvaluatableParser<CountCurrencyEvaluatableValue, decimal>
    {
        public static readonly LowercaseString FunctionName = "COUNTCURRENCY";

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

            Currency matchedCurrency;
            if (!CurrencyDatabase.TryGet(argumentParts[0], out matchedCurrency))
            {
                parsedValue = null;
                return false;
            }

            parsedValue = new CountCurrencyEvaluatableValue(matchedCurrency);
            return true;
        }
    }
}