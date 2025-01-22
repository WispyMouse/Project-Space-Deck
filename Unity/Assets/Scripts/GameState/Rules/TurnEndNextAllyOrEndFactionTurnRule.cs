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

    public class TurnEndNextAllyOrEndFactionTurnRule : Rule
    {
        public TurnEndNextAllyOrEndFactionTurnRule() : base(WellknownGameStateEvents.EntityTurnEnded)
        {
        }

        public override bool TryApplyRule(GameStateEventTrigger trigger, TriggerDirection direction, IGameStateMutator gameStateMutator, out List<GameStateChange> applications)
        {
            if (direction != TriggerDirection.After)
            {
                applications = null;
                return false;
            }

            if (gameStateMutator.EntityTurnTakerCalculator.TryGetNextEntityTurn(gameStateMutator, out Entity nextTurnSameFaction))
            {
                applications = new List<GameStateChange>() { new StartEntityTurn(nextTurnSameFaction) };
                return true;
            }

            applications = new List<GameStateChange>() { new EndCurrentFactionTurn() };
            return true;
        }
    }
}