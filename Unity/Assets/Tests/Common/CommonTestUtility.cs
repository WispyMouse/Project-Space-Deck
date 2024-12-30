namespace SpaceDeck.Tests.EditMode
{
    using SpaceDeck.GameState.Execution;
    using SpaceDeck.Models.Databases;
    using SpaceDeck.Tokenization.Processing;
    using SpaceDeck.UX.AssetLookup;

    public static class CommonTestUtility
    {
        public static void TearDownDatabases()
        {
            AllDatabases.ClearAllDatabases();
        }
    }
}
