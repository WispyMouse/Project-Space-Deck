namespace SpaceDeck.GameState.Minimum
{
    using SpaceDeck.Utility.Minimum;
    using SpaceDeck.Utility.Wellknown;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class EntityTurnTakerCalculator
    {
        public readonly List<Entity> EntitiesToTakeTurn = new List<Entity>();
        public int CurrentEntityTurnIndex = 0;

        public EntityTurnTakerCalculator(IGameStateMutator gameState, decimal factionId)
        {
            foreach (Entity curEntity in gameState.GetAllEntities())
            {
                if (curEntity.GetQuality(WellknownQualities.Faction, -1) == factionId)
                {
                    this.EntitiesToTakeTurn.Add(curEntity);
                }
            }
        }

        public bool TryGetCurrentEntityTurn(IGameStateMutator gameState, out Entity currentTurn)
        {
            if (this.EntitiesToTakeTurn.Count <= this.CurrentEntityTurnIndex)
            {
                currentTurn = null;
                return false;
            }

            if (!gameState.EntityIsAlive(this.EntitiesToTakeTurn[this.CurrentEntityTurnIndex]))
            {
                return this.TryGetNextEntityTurn(gameState, out currentTurn);
            }

            currentTurn = this.EntitiesToTakeTurn[this.CurrentEntityTurnIndex];
            return true;
        }

        public bool TryGetNextEntityTurn(IGameStateMutator gameState, out Entity nextTurn)
        {
            this.CurrentEntityTurnIndex++;

            if (this.EntitiesToTakeTurn.Count <= this.CurrentEntityTurnIndex)
            {
                nextTurn = null;
                return false;
            }

            if (!gameState.EntityIsAlive(this.EntitiesToTakeTurn[this.CurrentEntityTurnIndex]))
            {
                return this.TryGetNextEntityTurn(gameState, out nextTurn);
            }

            nextTurn = this.EntitiesToTakeTurn[this.CurrentEntityTurnIndex];
            return true;
        }
    }
}