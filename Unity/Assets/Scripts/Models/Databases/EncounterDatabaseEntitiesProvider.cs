namespace SpaceDeck.Models.Databases
{
    using System.Collections.Generic;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Models.Instances;
    using SpaceDeck.Models.Prototypes;
    using SpaceDeck.Utility.Logging;
    using SpaceDeck.Utility.Minimum;
    using SpaceDeck.Utility.Wellknown;

    public class EncounterDatabaseEntitiesProvider : IEncounterEntitiesProvider
    {
        public static readonly EncounterDatabaseEntitiesProvider Instance = new EncounterDatabaseEntitiesProvider();

        private EncounterDatabaseEntitiesProvider()
        {

        }

        public IEnumerable<Entity> GetEntities(IEnumerable<LowercaseStringSet> entityIds, RandomDecider<EnemyPrototype> decider = null)
        {
            if (decider == null)
            {
                decider = new RandomDecider<EnemyPrototype>();
            }

            List<Entity> entities = new List<Entity>();

            foreach (LowercaseStringSet entityId in entityIds)
            {
                if (!EnemyDatabase.TryGetInstanceWithTagsOrId(entityId, out EnemyInstance instance, decider))
                {
                    Logging.DebugLog(WellknownLoggingLevels.Error,
                        WellknownLoggingCategories.EnemyDatabase,
                        $"Failed to get enemy for tags '{entityId}'.");
                    continue;
                }
                entities.Add(instance);
            }

            return entities;
        }
    }
}