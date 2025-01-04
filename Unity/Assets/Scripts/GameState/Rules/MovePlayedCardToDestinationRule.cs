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

    public class MovePlayedCardToDestinationRule : Rule
    {
        public MovePlayedCardToDestinationRule() : base(triggerOneventId: WellknownGameStateEvents.CardPlayed)
        {

        }

        public override bool TryApplyRule(GameStateEventTrigger trigger, IGameStateMutator gameStateMutator, out List<GameStateChange> applications)
        {
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