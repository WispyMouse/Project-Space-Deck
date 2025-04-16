namespace SpaceDeck.GameState.Rules
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using SpaceDeck.GameState.Changes;
    using SpaceDeck.GameState.Execution;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Tokenization.Evaluatables;
    using SpaceDeck.Tokenization.Minimum.Context;
    using SpaceDeck.Utility.Logging;
    using SpaceDeck.Utility.Wellknown;
    using static SpaceDeck.GameState.Minimum.GameStateEventTrigger;

    public class PlayerTurnStartDrawCardsRule : Rule
    {
        public readonly INumericEvaluatableValue AmountToDraw;

        public PlayerTurnStartDrawCardsRule(INumericEvaluatableValue amountToDraw) : base(WellknownGameStateEvents.EntityTurnStarted)
        {
            this.AmountToDraw = amountToDraw;
        }

        public override bool TryApplyRule(GameStateEventTrigger trigger, TriggerDirection direction, IGameStateMutator gameStateMutator, out List<GameStateChange> applications)
        {
            if (direction != TriggerDirection.After)
            {
                applications = null;
                return false;
            }

            if (!gameStateMutator.EntityTurnTakerCalculator.TryGetCurrentEntityTurn(gameStateMutator, out Entity currentTurn))
            {
                applications = null;
                return false;
            }

            if (currentTurn.Qualities.GetNumericQuality(WellknownQualities.Faction) != WellknownFactions.Player)
            {
                applications = null;
                return false;
            }

            if (!this.AmountToDraw.TryEvaluate(gameStateMutator, out decimal amountToDraw))
            {
                Logging.DebugLog(WellknownLoggingLevels.Error, WellknownLoggingCategories.Rule, $"Amount to draw could not be resolved, so cannot draw hand.");
                applications = null;
                return false;
            }

            Logging.DebugLog(WellknownLoggingLevels.DebugVerbose, WellknownLoggingCategories.Rule, $"Attempting to draw starting hand of {amountToDraw} cards.");
            applications = new List<GameStateChange>()
            {
                new DrawCard(amountToDraw)
            };
            return true;
        }
    }
}