namespace SpaceDeck.GameState.Rules
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using SpaceDeck.GameState.Changes;
    using SpaceDeck.GameState.Execution;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Tokenization.Minimum.Context;
    using SpaceDeck.Utility.Logging;
    using SpaceDeck.Utility.Wellknown;
    using static SpaceDeck.GameState.Minimum.GameStateEventTrigger;

    public class NonPlayerEntityEndsTurnRule : Rule
    {
        public NonPlayerEntityEndsTurnRule() : base(WellknownGameStateEvents.EntityTurnStarted, -1)
        {
        }

        public override bool TryApplyRule(GameStateEventTrigger trigger, TriggerDirection direction, IGameStateMutator gameStateMutator, out List<GameStateChange> applications)
        {
            if (direction != TriggerDirection.After)
            {
                applications = null;
                return false;
            }

            if (!gameStateMutator.EntityTurnTakerCalculator.TryGetCurrentEntityTurn(gameStateMutator, out Entity currentTurnTaker))
            {
                applications = null;
                return false;
            }

            // Ignore the player, they can end their turn on their own
            if (gameStateMutator.GetNumericQuality(currentTurnTaker, WellknownQualities.Faction) == WellknownFactions.Player)
            {
                applications = null;
                return false;
            }

            applications = new List<GameStateChange>();
            applications.Add(new EndCurrentEntityTurn());
            return true;
        }
    }
}