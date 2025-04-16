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

    public class EncounterStartPlayerTurnRule : Rule
    {
        public EncounterStartPlayerTurnRule() : base(WellknownGameStateEvents.EncounterStart, -10)
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

            if (gameStateMutator.FactionTurnTakerCalculator.FactionsToTakeTurn.Count == 0)
            {
                Logging.DebugLog(WellknownLoggingLevels.Error, WellknownLoggingCategories.Rule, $"There are no factions. Could not start the first faction's turn.");
                applications = null;
                return false;
            }

            decimal nextTurn = gameStateMutator.FactionTurnTakerCalculator.GetCurrentFaction();

            applications = new List<GameStateChange>()
            {
                new StartFactionTurn(nextTurn)
            };

            return true;
        }
    }
}