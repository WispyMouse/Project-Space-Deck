namespace SpaceDeck.GameState.Minimum
{
    using SpaceDeck.Utility.Minimum;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class EntityTurnTakerCalculator
    {
        public readonly List<Entity> EntitiesToTakeTurn = new List<Entity>();
        public int NextEntityTurnIndex = 0;

        public EntityTurnTakerCalculator(IGameStateMutator gameState, decimal factionId)
        {

        }

        public bool TryGetCurrentEntityTurn(IGameStateMutator gameState, out Entity currentTurn)
        {
            if (this.EntitiesToTakeTurn.Count < this.NextEntityTurnIndex)
            {
                currentTurn = null;
                return false;
            }

            if (!gameState.EntityIsAlive(this.EntitiesToTakeTurn[this.NextEntityTurnIndex]))
            {
                this.NextEntityTurnIndex++;
                return this.TryGetCurrentEntityTurn(gameState, out currentTurn);
            }

            currentTurn = this.EntitiesToTakeTurn[this.NextEntityTurnIndex];
            return true;
        }

        public bool TryGetNextEntityTurn(IGameStateMutator gameState, out Entity nextTurn)
        {
            if (this.EntitiesToTakeTurn.Count < this.NextEntityTurnIndex)
            {
                nextTurn = null;
                return false;
            }

            if (!gameState.EntityIsAlive(this.EntitiesToTakeTurn[this.NextEntityTurnIndex]))
            {
                return this.TryGetNextEntityTurn(gameState, out nextTurn);
            }

            nextTurn = this.EntitiesToTakeTurn[this.NextEntityTurnIndex];
            this.NextEntityTurnIndex++;
            return true;
        }
    }
}