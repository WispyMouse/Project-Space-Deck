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
    using static SpaceDeck.GameState.Minimum.GameStateEventTrigger;

    public class EncounterStartPlayerTurnRule : Rule
    {
        public EncounterStartPlayerTurnRule() : base(WellknownGameStateEvents.EncounterStart)
        {
        }

        public override bool TryApplyRule(GameStateEventTrigger trigger, TriggerDirection direction, IGameStateMutator gameStateMutator, out List<GameStateChange> applications)
        {
            if (direction != TriggerDirection.After)
            {
                applications = null;
                return false;
            }

            gameStateMutator.FactionTurnTakerCalculator = new FactionTurnTakerCalculator(gameStateMutator);
            decimal nextTurn = gameStateMutator.FactionTurnTakerCalculator.GetCurrentFaction();

            applications = new List<GameStateChange>()
            {
                new StartFactionTurn(nextTurn)
            };

            return true;
        }
    }
}