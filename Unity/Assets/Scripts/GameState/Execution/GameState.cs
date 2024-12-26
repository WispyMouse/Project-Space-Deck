namespace SpaceDeck.GameState.Execution
{
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Utility.Minimum;
    using SpaceDeck.Utility.Wellknown;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Describes the state of a game.
    /// 
    /// This is meant to be entirely hollistic, holding information
    /// about every character, decision, card, deck, battle, etc.
    /// 
    /// This is not necessarily the current game's state. This object
    /// can be cloned, allowing for theoretical branches of the game's
    /// state in order to show certain properties and values that might
    /// exist after something is executed.
    /// </summary>
    public class GameState : IGameStateMutator
    {
        public EncounterState CurrentEncounterState = new EncounterState();
        public readonly List<Entity> PersistentEntities = new List<Entity>();
        public readonly PendingResolveStack PendingResolves = new PendingResolveStack();

        public EntityTurnTakerCalculator EntityTurnTakerCalculator { get; set; }
        public FactionTurnTakerCalculator FactionTurnTakerCalculator { get; set; }

        public List<Entity> AllEntities
        {
            get
            {
                // TODO: This is pretty horrible (I wrote it!)! It has a lot of allocating the
                // same list repeatedly. We should revisit this soon.
                List<Entity> allEntities = new List<Entity>(PersistentEntities);
                allEntities.AddRange(this.CurrentEncounterState.EncounterEnemies);
                return allEntities;
            }
        }

        public GameState()
        {

        }

        public void SetQuality(Entity entity, LowercaseString index, decimal toValue)
        {
            entity.SetQuality(index, toValue);
        }

        public decimal GetQuality(Entity entity, LowercaseString index, decimal defaultValue = 0)
        {
            return entity.GetQuality(index, defaultValue);
        }

        public void RemoveEntity(Entity entity)
        {
            this.PersistentEntities.Remove(entity);
            this.CurrentEncounterState.EncounterEnemies.Remove(entity);
        }

        public bool EntityIsAlive(Entity entity)
        {
            return this.PersistentEntities.Contains(entity) || (this.CurrentEncounterState?.EncounterEnemies.Contains(entity) ?? false);
        }

        public IReadOnlyList<Entity> GetAllEntities()
        {
            return this.AllEntities;
        }

        public void AddEncounterEntity(Entity toAdd)
        {
            this.CurrentEncounterState?.EncounterEnemies.Add(toAdd);
        }

        public void AddPersistentEntity(Entity toAdd)
        {
            this.PersistentEntities.Add(toAdd);
        }

        public void StartFactionTurn(decimal factionId)
        {
            this.TriggerAndStack(new GameStateEventTrigger(Utility.Wellknown.WellknownGameStateEvents.FactionTurnStarted));
        }

        public void StartEntityTurn(Entity toStart)
        {
            this.TriggerAndStack(new GameStateEventTrigger(Utility.Wellknown.WellknownGameStateEvents.EntityTurnStarted));
        }

        public void EndCurrentEntityTurn()
        {
            this.TriggerAndStack(new GameStateEventTrigger(Utility.Wellknown.WellknownGameStateEvents.EntityTurnEnded));
        }

        public void EndCurrentFactionTurn()
        {
            this.TriggerAndStack(new GameStateEventTrigger(Utility.Wellknown.WellknownGameStateEvents.FactionTurnEnded));
        }

        public void TriggerAndStack(GameStateEventTrigger trigger)
        {
            List<GameStateChange> appliedRules = RuleReference.GetAppliedRules(this, trigger);
            foreach (GameStateChange change in appliedRules)
            {
                this.PendingResolves.Push(change);
            }
        }

        public bool TryGetNextResolve(out IResolve currentResolve)
        {
            if (!this.PendingResolves.TryGetNextResolve(out currentResolve))
            {
                return false;
            }

            return true;
        }

        public void StartEncounter(EncounterState encounter)
        {
            this.CurrentEncounterState = encounter;
            this.TriggerAndStack(new GameStateEventTrigger(WellknownGameStateEvents.EncounterStart));
        }
    }
}