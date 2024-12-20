namespace SpaceDeck.GameState.Minimum
{
    using SpaceDeck.Utility.Minimum;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a list of <see cref="GameStateChange"/>.
    /// This can be used to represent what will happen when
    /// an entire effect resolves.
    /// </summary>
    public class GameStateDelta : IGameStateMutator
    {
        public enum EntityDestination
        {
            Removed,
            Encounter,
            Persistent
        }

        public readonly IGameStateMutator BaseGameState;
        public readonly List<GameStateChange> Changes = new List<GameStateChange>();

        public readonly Dictionary<Entity, Dictionary<LowercaseString, decimal>> DeltaValues = new Dictionary<Entity, Dictionary<LowercaseString, decimal>>();
        public readonly Dictionary<Entity, EntityDestination> EntityDestinationChanges = new Dictionary<Entity, EntityDestination>();

        public GameStateDelta(IGameStateMutator baseGameState)
        {
            this.BaseGameState = baseGameState;
        }

        public void SetQuality(Entity entity, LowercaseString index, decimal toValue)
        {
            if (!this.DeltaValues.TryGetValue(entity, out Dictionary<LowercaseString, decimal> entityAttributes))
            {
                entityAttributes = new Dictionary<LowercaseString, decimal>();
                this.DeltaValues.Add(entity, entityAttributes);
            }

            if (!entityAttributes.ContainsKey(index))
            {
                entityAttributes.Add(index, toValue);
            }
            else
            {
                entityAttributes[index] = toValue;
            }
        }

        public decimal GetQuality(Entity entity, LowercaseString index, decimal defaultValue = 0)
        {
            if (this.DeltaValues.TryGetValue(entity, out Dictionary<LowercaseString, decimal> entityAttributes) && entityAttributes.TryGetValue(index, out decimal existingValue))
            {
                return existingValue;
            }

            return entity.GetQuality(index, defaultValue);
        }

        public void RemoveEntity(Entity entity)
        {
            if (this.EntityDestinationChanges.ContainsKey(entity))
            {
                this.EntityDestinationChanges[entity] = EntityDestination.Removed;
            }
            else
            {
                this.EntityDestinationChanges.Add(entity, EntityDestination.Removed);
            }
        }

        public bool EntityIsAlive(Entity entity)
        {
            if (this.EntityDestinationChanges.TryGetValue(entity, out EntityDestination destination))
            {
                return destination != EntityDestination.Removed;
            }

            return this.BaseGameState.EntityIsAlive(entity);
        }

        public IReadOnlyList<Entity> GetAllEntities()
        {
            List<Entity> entities = new List<Entity>();
            entities.AddRange(this.BaseGameState.GetAllEntities());

            foreach (Entity movedEntity in this.EntityDestinationChanges.Keys)
            {
                switch (EntityDestinationChanges[movedEntity])
                {
                    case EntityDestination.Removed:
                        entities.Remove(movedEntity);
                        break;
                    case EntityDestination.Persistent:
                    case EntityDestination.Encounter:
                        if (!entities.Contains(movedEntity))
                        {
                            entities.Add(movedEntity);
                        }
                        break;
                }
            }

            return entities;
        }

        public void AddEncounterEntity(Entity toAdd)
        {
            if (this.EntityDestinationChanges.ContainsKey(toAdd))
            {
                this.EntityDestinationChanges[toAdd] = EntityDestination.Encounter;
            }
            else
            {
                this.EntityDestinationChanges.Add(toAdd, EntityDestination.Persistent);
            }
        }

        public void AddPersistentEntity(Entity toAdd)
        {
            if (this.EntityDestinationChanges.ContainsKey(toAdd))
            {
                this.EntityDestinationChanges[toAdd] = EntityDestination.Encounter;
            }
            else
            {
                this.EntityDestinationChanges.Add(toAdd, EntityDestination.Persistent);
            }
        }
    }
}