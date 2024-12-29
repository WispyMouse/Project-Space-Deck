using System.Text.RegularExpressions;
using SFDDCards.Evaluation.Actual;
using SFDDCards.ImportModels;
using SpaceDeck.GameState.Minimum;
using SpaceDeck.Models.Databases;
using SpaceDeck.Models.Instances;
using SpaceDeck.UX.AssetLookup;

namespace SFDDCards.ScriptingTokens.EvaluatableValues
{
    public class CountCurrencyEvaluatableValue : INumericEvaluatableValue
    {
        public readonly string CurrencyToCount;

        public CountCurrencyEvaluatableValue(string currencyToCount)
        {
            this.CurrencyToCount = currencyToCount;
        }

        public bool TryEvaluateValue(CampaignContext campaignContext, TokenEvaluatorBuilder currentBuilder, out int evaluatedValue)
        {
            if (string.IsNullOrEmpty(this.CurrencyToCount))
            {
                GlobalUpdateUX.LogTextEvent.Invoke($"Told to count currency, but not provided an id.", GlobalUpdateUX.LogType.RuntimeError);
                evaluatedValue = 0;
                return false;
            }

            if (!CurrencyDatabase.TryGet(this.CurrencyToCount, out Currency foundCurrency))
            {
                GlobalUpdateUX.LogTextEvent.Invoke($"Failed to find currency model named '{this.CurrencyToCount}'.", GlobalUpdateUX.LogType.RuntimeError);
                evaluatedValue = 0;
                return false;
            }

            evaluatedValue = campaignContext._GetCurrencyCount(foundCurrency);
            return true;
        }

        public bool TryEvaluateDecimalValue(CampaignContext campaignContext, TokenEvaluatorBuilder currentBuilder, out decimal evaluatedValue)
        {
            bool returnValue = this.TryEvaluateValue(campaignContext, currentBuilder, out int evaluatedIntValue);
            evaluatedValue = evaluatedIntValue;
            return returnValue;
        }

        public string DescribeEvaluation()
        {
            return this.DescribeEvaluation(this);
        }

        public string DescribeEvaluation(CampaignContext campaignContext, TokenEvaluatorBuilder currentBuilder)
        {
            if (this.TryEvaluateValue(campaignContext, currentBuilder, out int evaluatedValue))
            {
                return $"{this.DescribeEvaluation()} ({evaluatedValue})";
            }
            return this.DescribeEvaluation();
        }

        public static bool TryGetCountCurrencyEvaluatableValue(string argument, out CountCurrencyEvaluatableValue output, bool allowNameMatch)
        {
            Match regexMatch = Regex.Match(argument, @"COUNTCURRENCY_(\w+)");

            if (regexMatch.Success)
            {
                string stackId = regexMatch.Groups[1].Value;
                output = new CountCurrencyEvaluatableValue(stackId);
                return true;
            }

            if (allowNameMatch && CurrencyDatabase.TryGet(argument, out Currency currency))
            {
                output = new CountCurrencyEvaluatableValue(currency.Id.ToString());
                return true;
            }

            output = null;
            return false;
        }

        public string GetScriptingTokenText()
        {
            return $"COUNTCURRENCY_{this.CurrencyToCount}";
        }

        public string DescribeEvaluation(IEvaluatableValue<int> topValue)
        {
            Currency currency = CurrencyDatabase.Get(this.CurrencyToCount);

            if (topValue == this)
            {
                return $"1 x {SpriteLookup.GetNameAndMaybeIcon(currency)}";
            }

            return $"{SpriteLookup.GetNameAndMaybeIcon(currency)}";
        }
    }
}