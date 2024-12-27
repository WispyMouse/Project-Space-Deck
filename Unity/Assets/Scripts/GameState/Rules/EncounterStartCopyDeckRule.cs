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

    public class EncounterStartCopyDeckRule : Rule
    {
        public EncounterStartCopyDeckRule() : base(WellknownGameStateEvents.EncounterStart, priorityOrder: 1)
        {
        }

        public override bool TryApplyRule(GameStateEventTrigger trigger, IGameStateMutator gameStateMutator, out List<GameStateChange> applications)
        {
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