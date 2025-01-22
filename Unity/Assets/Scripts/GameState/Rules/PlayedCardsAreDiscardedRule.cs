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
    using SpaceDeck.Utility.Wellknown;
    using static SpaceDeck.GameState.Minimum.GameStateEventTrigger;

    public class PlayedCardsAreDiscardedRule : Rule
    {
        public PlayedCardsAreDiscardedRule() : base(triggerOneventId: WellknownGameStateEvents.CardPlayed, priorityOrder: 2)
        {

        }

        public override bool TryApplyRule(GameStateEventTrigger trigger, TriggerDirection direction, IGameStateMutator gameStateMutator, out List<GameStateChange> applications)
        {
            if (direction != TriggerDirection.After)
            {
                applications = null;
                return false;
            }

            if (trigger.ProccingCard != null)
            {
                applications = new List<GameStateChange>()
                {
                    new SetStringQuality(trigger.BasedOnTarget, trigger.ProccingCard, WellknownQualities.Destination, WellknownZones.Discard)
                };
                return true;
            }

            applications = null;
            return false;
        }
    }
}