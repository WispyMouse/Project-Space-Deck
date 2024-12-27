namespace SpaceDeck.Tests.EditMode
{
    using SpaceDeck.GameState.Execution;
    using SpaceDeck.Models.Databases;
    using SpaceDeck.Tokenization.Processing;

    public static class CommonTestUtility
    {
        public static void TearDownDatabases()
        {
            ScriptingCommandReference.Clear();
            RuleReference.ClearRules();
            CardDatabase.ClearDatabase();
        }
    }
}
