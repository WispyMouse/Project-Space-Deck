namespace SpaceDeck.Tokenization.Processing
{
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.Tokenization.Evaluatables;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// A lookup table of all <see cref="IEvalautables"/>.
    /// </summary>
    public static class EvaluatablesReference
    {
        private readonly static List<EvaluatableParser> parsers = new List<EvaluatableParser>();
        private readonly static Dictionary<LowercaseString, EvaluatableParser> parserHistory = new Dictionary<LowercaseString, EvaluatableParser>();

        public static void SubscribeEvaluatable(EvaluatableParser parser)
        {
            parsers.Add(parser); 
        }

        public static void Clear()
        {
            parserHistory.Clear();
            parsers.Clear();
        }

        public static bool TryGetNumericEvaluatableValue(LowercaseString argument, out INumericEvaluatableValue numericEvaluatable)
        {
            if (!TrySelectParser(argument, out EvaluatableParser parser, out IEvaluatableValue evaluatedValue))
            {
                numericEvaluatable = null;
                return false;
            }

            if (!(evaluatedValue is INumericEvaluatableValue numericEvaluatableValue))
            {
                numericEvaluatable = null;
                return false;
            }

            numericEvaluatable = numericEvaluatableValue;
            return true;
        }

        public static bool TrySelectParser(LowercaseString argument, out EvaluatableParser parser, out IEvaluatableValue parsedEvaluatableValue)
        {
            if (parserHistory.TryGetValue(argument, out parser) && parser.TryParse(argument, out parsedEvaluatableValue))
            {
                return true;
            }

            foreach (EvaluatableParser tryingParser in parsers)
            {
                if (tryingParser.TryParse(argument, out parsedEvaluatableValue))
                {
                    parser = tryingParser;
                    return true;
                }
            }

            parsedEvaluatableValue = null;
            return false;
        }
    }
}