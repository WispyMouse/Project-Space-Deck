namespace SpaceDeck.GameState.Minimum
{
    using SpaceDeck.Utility.Minimum;
    using SpaceDeck.Utility.Wellknown;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class FactionTurnTakerCalculator
    {
        public readonly List<decimal> FactionsToTakeTurn = new List<decimal>();
        protected int CurrentFactionTurnIndex = 0;

        public FactionTurnTakerCalculator(IGameStateMutator gameState)
        {
            HashSet<decimal> factions = new HashSet<decimal>();

            foreach (Entity curEntity in gameState.GetAllEntities())
            {
                factions.Add(curEntity.Qualities.GetNumericQuality(WellknownQualities.Faction, WellknownFactions.UnknownFaction));
            }

            List<decimal> factionsToTakeTurn = new List<decimal>(factions);
            factionsToTakeTurn.Sort();
            this.FactionsToTakeTurn = factionsToTakeTurn;
        }

        public decimal GetCurrentFaction()
        {
            return this.FactionsToTakeTurn[this.CurrentFactionTurnIndex];
        }

        public decimal GetNextFactionTurn(IGameStateMutator gameState)
        {
            // If there are some how no factions, just report the faction that's already active.
            if (this.FactionsToTakeTurn.Count == 0)
            {
                return this.CurrentFactionTurnIndex;
            }

            // If there's only one faction, it must be their turn
            if (this.FactionsToTakeTurn.Count == 1)
            {
                return this.FactionsToTakeTurn[0];
            }

            int nextFactionIndex = CurrentFactionTurnIndex + 1;

            // Wrap around ihis is the last faction index
            if (nextFactionIndex >= this.FactionsToTakeTurn.Count)
            {
                nextFactionIndex = 0;
            }

            return this.FactionsToTakeTurn[nextFactionIndex];
        }

        public void SetCurrentTurnTaker(decimal currentTurn)
        {
            int turnIndex = FactionsToTakeTurn.IndexOf(currentTurn);
            if (turnIndex == -1)
            {
                // TODO: LOG FAILURE
            }
            else
            {
                this.CurrentFactionTurnIndex = turnIndex;
            }
        }
    }
}