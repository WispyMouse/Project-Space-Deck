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
    using SpaceDeck.Tokenization.Evaluatables;

    public static class AllDatabases
    {
        public static void ClearAllDatabases()
        {
            CardDatabase.ClearDatabase();
            CurrencyDatabase.ClearDatabase();
            ElementDatabase.ClearDatabase();
            EncounterDatabase.ClearDatabase();
            StatusEffectDatabase.ClearDatabase();
            RouteDatabase.ClearDatabase();
            RewardDatabase.ClearDatabase();
            ScriptingCommandReference.Clear();
            RuleReference.ClearRules();
            EvaluatablesReference.Clear();
        }

        public static void LinkAllDatabase()
        {
            CardDatabase.LinkTokens();
            StatusEffectDatabase.LinkTokens();
        }
    }
}