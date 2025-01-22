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

    public class EncounterStartCopyDeckRule : Rule
    {
        public EncounterStartCopyDeckRule() : base(WellknownGameStateEvents.EncounterStart, priorityOrder: 5)
        {
        }

        public override bool TryApplyRule(GameStateEventTrigger trigger, TriggerDirection direction, IGameStateMutator gameStateMutator, out List<GameStateChange> applications)
        {
            if (direction != TriggerDirection.After)
            {
                applications = null;
                return false;
            }

            applications = new List<GameStateChange>();

            // Stack shuffling the deck first, so it executes last
            applications.Add(new ShuffleDeck());

            foreach (CardInstance card in gameStateMutator.GetCardsInZone(WellknownZones.Campaign))
            {
                applications.Add(new AddCard(card, WellknownZones.Deck));
            }

            return true;
        }
    }
}