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

    public class EntityActsOnIntentRule : Rule
    {
        public EntityActsOnIntentRule() : base(WellknownGameStateEvents.EntityTurnStarted, 0)
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

            // If this has an intent, act upon it
            // Something can validly not have an intent; the player generally should never have one
            Intent currentIntent = currentTurnTaker.CurrentIntent;
            if (currentIntent == null)
            {
                
                Logging.DebugLog(WellknownLoggingLevels.DebugVerbose, WellknownLoggingCategories.Rule, $"Entity {gameStateMutator.GetStringQuality(currentTurnTaker, WellknownQualities.Name)} has no intent to act upon.");
                applications = null;
                return false;
            }

            Logging.DebugLog(WellknownLoggingLevels.DebugVerbose, WellknownLoggingCategories.Rule, $"Entity {gameStateMutator.GetStringQuality(currentTurnTaker, WellknownQualities.Name)} is acting on intent {currentIntent.Describe()}.");
            applications = new List<GameStateChange>();
            applications.Add(new SetEntityIntent(currentTurnTaker, null));
            applications.AddRange(new List<GameStateChange>(currentTurnTaker.CurrentIntent.ActOnIntent(gameStateMutator)));

            return true;
        }
    }
}