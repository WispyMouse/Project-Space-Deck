namespace SpaceDeck.Models.Databases
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Models.Imports;
    using SpaceDeck.Models.Instances;
    using SpaceDeck.Models.Prototypes;
    using SpaceDeck.Tokenization.Evaluatables;
    using SpaceDeck.Utility.Logging;
    using SpaceDeck.Utility.Minimum;
    using SpaceDeck.Utility.Wellknown;

    public class CurrencyDatabase
    {
        public static Dictionary<LowercaseString, Currency> CurrencyData { get; private set; } = new Dictionary<LowercaseString, Currency>();
        public const string StartCostString = "[cost:";


        public static void AddCurrencyToDatabase(CurrencyImport toImport)
        {
            AddCurrencyToDatabase(toImport.GetCurrency());
        }

        public static void AddCurrencyToDatabase(Currency toImport)
        {
            CurrencyData.Add(toImport.Id, toImport);
            Logging.DebugLog(WellknownLoggingLevels.ImportComplete,
                WellknownLoggingCategories.DatabaseImportCompletion,
                $"Currency: Imported {toImport.Id} successfully.");
        }

        public static Currency Get(LowercaseString id)
        {
            return CurrencyData[id];
        }

        public static bool TryGet(LowercaseString id, out Currency found)
        {
            return CurrencyData.TryGetValue(id, out found);
        }

        public static void ClearDatabase()
        {
            CurrencyData.Clear();
        }

        public static bool IsCost(LowercaseString cost)
        {
            return cost.Value.StartsWith("[cost:") && cost.Value.EndsWith("]");
        }

        public static IShopCost GetCost(LowercaseString cost)
        {
            Logging.DebugLog(WellknownLoggingLevels.DebugVerbose, WellknownLoggingCategories.EvaluatableEvaluation, $"Creating evaluation for '{cost.Value}'.");
            string afterCost = cost.Value.Substring(StartCostString.Length).TrimEnd(']');
            Logging.DebugLog(WellknownLoggingLevels.DebugVerbose, WellknownLoggingCategories.EvaluatableEvaluation, $"After cost is '{afterCost}'.");
            string currencyId = afterCost.Substring(0, afterCost.IndexOf(':'));
            Logging.DebugLog(WellknownLoggingLevels.DebugVerbose, WellknownLoggingCategories.EvaluatableEvaluation, $"Currency id is '{currencyId}'.");
            string evaluatableString = afterCost.Substring(currencyId.Length + 1);
            Logging.DebugLog(WellknownLoggingLevels.DebugVerbose, WellknownLoggingCategories.EvaluatableEvaluation, $"Evaluatable string is '{evaluatableString}'.");

            if (!CurrencyDatabase.TryGet(currencyId, out Currency currencyType))
            {
                Logging.DebugLog(WellknownLoggingLevels.Error, WellknownLoggingCategories.EvaluatableEvaluation, $"Failed to find currency '{currencyId}'.");
            }

            if (!EvaluatablesReference.TryGetNumericEvaluatableValue(evaluatableString, out INumericEvaluatableValue evaluatableValue))
            {
                Logging.DebugLog(WellknownLoggingLevels.Error, WellknownLoggingCategories.EvaluatableEvaluation, $"Failed to evaluate substring '{evaluatableString}'.");
            }
            return new ShopCost(currencyType, new IntegerNumericEvaluatableValue(evaluatableValue));
        }
    }
}