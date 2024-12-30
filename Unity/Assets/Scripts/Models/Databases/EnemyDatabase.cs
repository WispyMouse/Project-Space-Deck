namespace SpaceDeck.Models.Databases
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Models.Imports;
    using SpaceDeck.Models.Instances;
    using SpaceDeck.Models.Prototypes;
    using SpaceDeck.Utility.Minimum;

    public static class EnemyDatabase
    {
        public static Dictionary<LowercaseString, EnemyPrototype> EnemyData { get; set; } = new Dictionary<LowercaseString, EnemyPrototype>();

        public static void ClearDatabase()
        {
            EnemyData.Clear();
        }

        public static void AddEnemy(EnemyImport import)
        {
            AddEnemy(import.GetPrototype());
        }

        public static void AddEnemy(EnemyPrototype prototype)
        {
            EnemyData.Add(prototype.Id, prototype);
        }

        public static EnemyInstance GetInstance(LowercaseString id)
        {
            return new EnemyInstance(EnemyData[id]);
        }
    }
}