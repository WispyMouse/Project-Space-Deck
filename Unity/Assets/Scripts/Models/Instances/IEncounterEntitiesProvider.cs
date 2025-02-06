namespace SpaceDeck.Models.Instances
{
    using System.Collections;
    using System.Collections.Generic;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.GameState.Deltas;
    using SpaceDeck.Models.Prototypes;
    using static SpaceDeck.GameState.Minimum.GameStateEventTrigger;
    using SpaceDeck.Tokenization.Minimum.Questions;
    using SpaceDeck.Utility.Logging;
    using SpaceDeck.Utility.Wellknown;
    using SpaceDeck.Utility.Minimum;

    public interface IEncounterEntitiesProvider
    {
        IEnumerable<Entity> GetEntities(IEnumerable<LowercaseStringSet> entityIds, RandomDecider<EnemyPrototype> decider = null);
    }
}