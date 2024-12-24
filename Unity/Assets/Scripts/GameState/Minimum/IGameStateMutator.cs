namespace SpaceDeck.GameState.Minimum
{
    using SpaceDeck.Utility.Minimum;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public interface IGameStateMutator
    {
        EntityTurnTakerCalculator EntityTurnTakerCalculator { get; set; }
        FactionTurnTakerCalculator FactionTurnTakerCalculator { get; set; }

        void SetQuality(Entity entity, LowercaseString index, decimal toValue);
        decimal GetQuality(Entity entity, LowercaseString index, decimal defaultValue = 0);
        void AddEncounterEntity(Entity toAdd);
        void AddPersistentEntity(Entity toAdd);
        void RemoveEntity(Entity entity);
        bool EntityIsAlive(Entity entity);
        IReadOnlyList<Entity> GetAllEntities();

        void StartFactionTurn(decimal factionId);
        void EndCurrentFactionTurn();

        void StartEntityTurn(Entity toStart);
        void EndCurrentEntityTurn();
    }
}