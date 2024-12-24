namespace SpaceDeck.GameState.Rules
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using SpaceDeck.GameState.Changes;
    using SpaceDeck.GameState.Execution;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Tokenization.Minimum.Context;
    using SpaceDeck.Utility.Wellknown;

    public class EncounterStartPlayerTurnRule : Rule
    {
        public EncounterStartPlayerTurnRule() : base(WellknownGameStateEvents.EncounterStart)
        {
        }

        public override bool TryApplyRule(GameStateEventTrigger trigger, IGameStateMutator gameStateMutator, out List<GameStateChange> applications)
        {
            foreach (Entity curEntity in gameStateMutator.GetAllEntities())
            {
                if (curEntity.GetQuality(WellknownQualities.Faction, WellknownFactions.UnknownFaction) == WellknownFactions.Player)
                {
                    applications = new List<GameStateChange>()
                    {
                        new StartFactionTurn(WellknownFactions.Player)
                    };
                    return true;
                }
            }

            applications = new List<GameStateChange>();
            return false;
        }
    }
}