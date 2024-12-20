namespace SpaceDeck.GameState.Minimum
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public interface IGameStateMutator
    {
        void SetQuality(Entity entity, string index, decimal toValue);
        decimal GetQuality(Entity entity, string index, decimal defaultValue = 0);
        void AddEncounterEntity(Entity toAdd);
        void AddPersistentEntity(Entity toAdd);
        void RemoveEntity(Entity entity);
        bool EntityIsAlive(Entity entity);
        IReadOnlyList<Entity> GetAllEntities();
    }
}