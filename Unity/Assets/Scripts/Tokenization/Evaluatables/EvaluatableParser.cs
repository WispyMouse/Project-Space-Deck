namespace SpaceDeck.Tokenization.Evaluatables
{
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Utility.Minimum;
    using System.Collections.Generic;

    public abstract class EvaluatableParser
    {
        public const string FunctionParameterStart = "(";
        public const string FunctionParameterSeparator = ",";
        public const string FunctionParamterEnd = ")";

        public abstract bool TryParse(LowercaseString argument, out IEvaluatableValue parsedValue);

        public bool TryParseFunctionFromArgument(LowercaseString functionName, LowercaseString tokenTextString, out List<LowercaseString> tokenTextParts)
        {
            if (!tokenTextString.Value.StartsWith(functionName.Value + EvaluatableParser.FunctionParameterStart) || !tokenTextString.Value.EndsWith(EvaluatableParser.FunctionParamterEnd))
            {
                tokenTextParts = null;
                return false;
            }

            string substring = tokenTextString.Value.Substring(functionName.Value.Length + FunctionParameterStart.Length, tokenTextString.Value.Length - (functionName.Value.Length + FunctionParameterStart.Length + FunctionParamterEnd.Length));

            string[] separated = substring.Split(FunctionParameterSeparator.ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);
            tokenTextParts = new List<LowercaseString>();
            foreach (string split in separated)
            {
                tokenTextParts.Add(split);
            }

            return true;
        }
    }

    public abstract class EvaluatableParser<T,V> : EvaluatableParser where T : IEvaluatableValue<V>
    {
        public override bool TryParse(LowercaseString argument, out IEvaluatableValue parsedValue)
        {
            if (this.TryParse(argument, out IEvaluatableValue<V> boxedEvaluatedParsedValue))
            {
                parsedValue = boxedEvaluatedParsedValue;
                return true;
            }

            parsedValue = null;
            return false;
        }

        public abstract bool TryParse(LowercaseString argument, out IEvaluatableValue<V> parsedValue);
    }
}