namespace SpaceDeck.Models.Instances
{
    using System.Collections.Generic;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Models.Prototypes;
    using SpaceDeck.Utility.Minimum;

    public interface IEncounterEntitiesProvider
    {
        IEnumerable<Entity> GetEntities(IEnumerable<LowercaseStringSet> entityIds, RandomDecider<EnemyPrototype> decider = null);
    }
}