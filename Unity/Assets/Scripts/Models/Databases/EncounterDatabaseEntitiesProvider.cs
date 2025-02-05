namespace SpaceDeck.Models.Databases
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Models.Imports;
    using SpaceDeck.Models.Instances;
    using SpaceDeck.Models.Prototypes;
    using SpaceDeck.Utility.Minimum;

    public class EncounterDatabaseEntitiesProvider : IEncounterEntitiesProvider
    {
        public static readonly EncounterDatabaseEntitiesProvider Instance = new EncounterDatabaseEntitiesProvider();

        private EncounterDatabaseEntitiesProvider()
        {

        }

        public IEnumerable<Entity> GetEntities(IEnumerable<LowercaseString> entityIds)
        {
            List<Entity> entities = new List<Entity>();

            foreach (LowercaseString entityId in entityIds)
            {
                entities.Add(EnemyDatabase.GetInstance(entityId));
            }

            return entities;
        }
    }
}