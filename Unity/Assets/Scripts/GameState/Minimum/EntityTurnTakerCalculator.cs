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
        protected int CurrentEntityTurnIndex = 0;
        protected decimal FactionId = 0;

        public EntityTurnTakerCalculator(IGameStateMutator gameState, decimal factionId)
        {
            this.FactionId = factionId;

            foreach (Entity curEntity in gameState.GetAllEntities())
            {
                if (curEntity.Qualities.GetNumericQuality(WellknownQualities.Faction, -1) == factionId)
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
            int nextIndex = ++this.CurrentEntityTurnIndex;

            if (this.EntitiesToTakeTurn.Count <= nextIndex)
            {
                nextTurn = null;
                return false;
            }

            if (!gameState.EntityIsAlive(this.EntitiesToTakeTurn[nextIndex]))
            {
                return this.TryGetNextEntityTurn(gameState, out nextTurn);
            }

            nextTurn = this.EntitiesToTakeTurn[nextIndex];
            return true;
        }

        public void SetCurrentTurnTaker(Entity currentTurn)
        {
            int turnIndex = EntitiesToTakeTurn.IndexOf(currentTurn);
            if (turnIndex == -1)
            {
                // TODO: LOG FAILURE
            }
            else
            {
                this.CurrentEntityTurnIndex = turnIndex;
            }
        }

        public EntityTurnTakerCalculator Clone(IGameStateMutator gameState)
        {
            EntityTurnTakerCalculator newCalculator = new EntityTurnTakerCalculator(gameState, this.FactionId);
            newCalculator.CurrentEntityTurnIndex = this.CurrentEntityTurnIndex;
            newCalculator.EntitiesToTakeTurn.AddRange(this.EntitiesToTakeTurn);
            return newCalculator;
        }
    }
}