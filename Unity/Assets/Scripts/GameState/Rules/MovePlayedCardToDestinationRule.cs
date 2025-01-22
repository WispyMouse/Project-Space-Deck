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
    using SpaceDeck.Utility.Minimum;
    using SpaceDeck.Utility.Wellknown;
    using static SpaceDeck.GameState.Minimum.GameStateEventTrigger;

    public class MovePlayedCardToDestinationRule : Rule
    {
        public MovePlayedCardToDestinationRule() : base(triggerOneventId: WellknownGameStateEvents.CardPlayed)
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
                LowercaseString destination = gameStateMutator.GetStringQuality(trigger.ProccingCard, WellknownQualities.Destination, WellknownZones.Discard);

                applications = new List<GameStateChange>()
                {
                    new MoveCard(trigger.ProccingCard, destination)
                };
                return true;
            }

            applications = null;
            return false;
        }
    }
}