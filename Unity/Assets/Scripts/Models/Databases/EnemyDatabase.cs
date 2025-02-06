namespace SpaceDeck.Models.Databases
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Models.Imports;
    using SpaceDeck.Models.Instances;
    using SpaceDeck.Models.Prototypes;
    using SpaceDeck.Utility.Logging;
    using SpaceDeck.Utility.Minimum;
    using SpaceDeck.Utility.Wellknown;

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
            if (!EnemyData.ContainsKey(id))
            {
                Logging.DebugLog(WellknownLoggingLevels.Error,
                    WellknownLoggingCategories.DatabaseImportCompletion,
                    $"Failure to get enemy with id '{id}'.");
                return null;
            }

            return new EnemyInstance(EnemyData[id]);
        }

        public static bool TryGetInstanceWithTagsOrId(LowercaseStringSet tagsOrId, out EnemyInstance instance, RandomDecider<EnemyPrototype> decider = null)
        {
            if (decider == null)
            {
                decider = new RandomDecider<EnemyPrototype>();
            }

            if (tagsOrId.OnlyValue.HasValue && EnemyData.ContainsKey(tagsOrId.OnlyValue.Value))
            {
                // Use the GetInstance call instead of duplicating its logic
                instance = GetInstance(tagsOrId.OnlyValue.Value);

                // Should always be true, given that the EnemyData dictionary contains this key
                return instance != null;
            }

            List<EnemyPrototype> fittingPrototypes = new List<EnemyPrototype>();
            foreach (EnemyPrototype prototype in EnemyData.Values)
            {
                if (!prototype.Tags.Contains(tagsOrId))
                {
                    continue;
                }
                fittingPrototypes.Add(prototype);
            }

            if (fittingPrototypes.Count == 0)
            {
                Logging.DebugLog(WellknownLoggingLevels.Error,
                    WellknownLoggingCategories.DatabaseImportCompletion,
                    $"Failure to find enemy with tags: {tagsOrId}");
                instance = null;
                return false;
            }

            EnemyPrototype chosenPrototype = decider.ChooseRandomly(fittingPrototypes);

            // Reuse GetInstance
            instance = GetInstance(chosenPrototype.Id);

            // Should certainly not be null
            return instance != null;
        }
    }
}