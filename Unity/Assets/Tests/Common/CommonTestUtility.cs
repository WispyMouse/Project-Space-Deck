namespace SpaceDeck.Tests.EditMode
{
    using SpaceDeck.GameState.Execution;
    using SpaceDeck.Models.Databases;
    using SpaceDeck.Tokenization.Processing;
    using SpaceDeck.Utility.Minimum;
    using SpaceDeck.Utility.Wellknown;
    using SpaceDeck.UX.AssetLookup;

    public static class CommonTestUtility
    {
        public const string GreaterThanOrEqualToAscii = "\u2265";
        public const string LessThanOrEqualToAscii = "\u2264";

        public static void TearDownDatabases()
        {
            AllDatabases.ClearAllDatabases();
        }
    }
}
