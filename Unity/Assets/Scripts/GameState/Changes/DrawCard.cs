namespace SpaceDeck.GameState.Changes
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Tokenization.Evaluatables;
    using SpaceDeck.Utility.Logging;
    using SpaceDeck.Utility.Minimum;
    using SpaceDeck.Utility.Wellknown;

    public class DrawCard : GameStateChange, IChangeWithIntensity
    {
        public decimal Intensity { get; set; } = 0;
        public InitialIntensityPositivity Positivity => InitialIntensityPositivity.PositiveOrZero;

        public DrawCard(int amountToDraw) : base(NobodyTarget.Instance)
        {
            this.Intensity = amountToDraw;
        }

        public DrawCard(decimal amountToDraw) : this((int)Math.Floor(amountToDraw))
        {
        }

        public override void Apply(IGameStateMutator toApplyTo)
        {
            for (int ii = (int)toApplyTo.GetIntensity(this); ii > 0; ii--)
            {
                IReadOnlyList<CardInstance> cardsInDeck = toApplyTo.GetCardsInZone(WellknownZones.Deck);

                // If there are no cards in deck, we should attempt to reshuffle the discard into the deck
                if (cardsInDeck.Count == 0)
                {
                    Logging.DebugLog(WellknownLoggingLevels.DebugVerbose, WellknownLoggingCategories.Change, $"There are no cards in the deck currently. Shuffling...");
                    toApplyTo.ShuffleDiscardAndDeck();
                    cardsInDeck = toApplyTo.GetCardsInZone(WellknownZones.Deck);

                    if (cardsInDeck.Count == 0)
                    {
                        Logging.DebugLog(WellknownLoggingLevels.DebugVerbose, WellknownLoggingCategories.Change, $"After shuffling, there are zero cards in the deck.");
                        break;
                    }
                }

                CardInstance cardToMove = cardsInDeck[0];
                Logging.DebugLog(WellknownLoggingLevels.DebugVerbose, WellknownLoggingCategories.Change, $"Moving card '{cardToMove.Id}' to hand");
                toApplyTo.MoveCard(cardToMove, WellknownZones.Hand);
            }
        }

        public override string Describe()
        {
            throw new NotImplementedException();
        }
    }
}