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
        public int NextFactionTurnIndex = 0;

        public FactionTurnTakerCalculator(IGameStateMutator gameState)
        {
            HashSet<decimal> factions = new HashSet<decimal>();

            foreach (Entity curEntity in gameState.GetAllEntities())
            {
                factions.Add(curEntity.GetQuality(WellknownQualities.Faction, WellknownFactions.UnknownFaction));
            }

            List<decimal> factionsToTakeTurn = new List<decimal>(factions);
            factionsToTakeTurn.Sort();
            this.FactionsToTakeTurn = factionsToTakeTurn;
        }

        public decimal GetCurrentFaction()
        {
            return this.FactionsToTakeTurn[this.NextFactionTurnIndex];
        }

        public decimal GetNextFactionTurn(IGameStateMutator gameState)
        {
            // If there are some how no factions, just report the faction that's already active.
            if (this.FactionsToTakeTurn.Count == 0)
            {
                return this.NextFactionTurnIndex;
            }

            // If there's only one faction, it must be their turn
            if (this.FactionsToTakeTurn.Count == 1)
            {
                return this.FactionsToTakeTurn[0];
            }

            decimal nextFaction = this.FactionsToTakeTurn[this.NextFactionTurnIndex];

            this.NextFactionTurnIndex++;

            if (this.NextFactionTurnIndex >= this.FactionsToTakeTurn.Count)
            {
                this.NextFactionTurnIndex = 0;
            }

            return nextFaction;
        }
    }
}