namespace SpaceDeck.GameState.Changes
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using SpaceDeck.GameState.Changes.Targets;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Tokenization.Evaluatables;
    using SpaceDeck.Utility.Minimum;
    using SpaceDeck.Utility.Wellknown;

    public class DrawCard : GameStateChange
    {
        public readonly INumericEvaluatableValue AmountToDraw;

        public DrawCard(INumericEvaluatableValue amountToDraw) : base(NobodyTarget.Instance)
        {
            this.AmountToDraw = amountToDraw;
        }

        public override void Apply(IGameStateMutator toApplyTo)
        {
            decimal cardsToDraw = 0;
            if (!this.AmountToDraw.TryEvaluate(toApplyTo, out cardsToDraw))
            {
                // TODO log
                return;
            }
            for (int ii = (int)cardsToDraw; ii >= 0; ii--)
            {
                IReadOnlyList<CardInstance> cardsInDeck = toApplyTo.GetCardsInZone(WellknownZones.Deck);

                // If there are no cards in deck, we should attempt to reshuffle the discard into the deck
                if (cardsInDeck.Count == 0)
                {
                    foreach (CardInstance cardFromDiscard in new List<CardInstance>(toApplyTo.GetCardsInZone(WellknownZones.Discard)))
                    {
                        toApplyTo.MoveCard(cardFromDiscard, WellknownZones.Deck);
                    }
                    toApplyTo.ShuffleDeck();
                }

                if (cardsInDeck.Count == 0)
                {
                    break;
                }

                toApplyTo.MoveCard(cardsInDeck[0], WellknownZones.Hand);
            }
        }
    }
}