namespace SpaceDeck.Models.Databases
{
    using System.Collections;
    using System.Collections.Generic;
    using SpaceDeck.Models.Prototypes;
    using SpaceDeck.Models.Instances;
    using SpaceDeck.Utility.Minimum;
    using SpaceDeck.Tokenization.Processing;
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Models.Imports;

    public static class AllDatabases
    {
        public static void ClearAllDatabases()
        {
            CardDatabase.ClearDatabase();
            CurrencyDatabase.ClearDatabase();
            ElementDatabase.ClearDatabase();
            EncounterDatabase.ClearDatabase();
            StatusEffectDatabase.ClearDatabase();
            ScriptingCommandReference.Clear();
            RuleReference.ClearRules();
        }

        public static void LinkAllDatabase()
        {
            CardDatabase.LinkTokens();
            StatusEffectDatabase.LinkTokens();
        }
    }
}