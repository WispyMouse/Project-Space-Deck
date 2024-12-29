namespace SpaceDeck.Models.Databases
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Models.Imports;
    using SpaceDeck.Models.Instances;
    using SpaceDeck.Models.Prototypes;
    using SpaceDeck.Utility.Minimum;


    public class CurrencyDatabase
    {
        public static Dictionary<LowercaseString, Currency> CurrencyData { get; private set; } = new Dictionary<LowercaseString, Currency>();

        public static void AddCurrencyToDatabase(CurrencyImport toImport)
        {
            AddCurrencyToDatabase(toImport.GetCurrency());
        }

        public static void AddCurrencyToDatabase(Currency toImport)
        {
            CurrencyData.Add(toImport.Id, toImport);
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
    }
}