namespace SpaceDeck.GameState.Minimum
{
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a list of <see cref="GameStateChange"/>.
    /// This can be used to represent what will happen when
    /// an entire effect resolves.
    /// </summary>
    public class GameStateDelta : IGameStateMutator
    {
        public readonly IGameStateMutator BaseGameState;
        public readonly List<GameStateChange> Changes = new List<GameStateChange>();

        public readonly Dictionary<Entity, Dictionary<string, decimal>> DeltaValues = new Dictionary<Entity, Dictionary<string, decimal>>();
        public readonly List<Entity> RemovedEntities = new List<Entity>();

        public GameStateDelta(IGameStateMutator baseGameState)
        {
            this.BaseGameState = baseGameState;
        }

        public void SetQuality(Entity entity, string index, decimal toValue)
        {
            if (!this.DeltaValues.TryGetValue(entity, out Dictionary<string, decimal> entityAttributes))
            {
                entityAttributes = new Dictionary<string, decimal>();
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

        public decimal GetQuality(Entity entity, string index, decimal defaultValue = 0)
        {
            if (this.DeltaValues.TryGetValue(entity, out Dictionary<string, decimal> entityAttributes) && entityAttributes.TryGetValue(index, out decimal existingValue))
            {
                return existingValue;
            }

            return entity.GetQuality(index, defaultValue);
        }

        public void RemoveEntity(Entity entity)
        {
            this.RemovedEntities.Add(entity);
        }

        public bool EntityIsAlive(Entity entity)
        {
            if (this.RemovedEntities.Contains(entity))
            {
                return false;
            }

            return this.BaseGameState.EntityIsAlive(entity);
        }

        public IReadOnlyList<Entity> GetAllEntities()
        {
            List<Entity> entities = new List<Entity>();
            entities.AddRange(this.BaseGameState.GetAllEntities());

            foreach (Entity removedEntity in this.RemovedEntities)
            {
                entities.Remove(removedEntity);
            }

            return entities;
        }
    }
}