namespace SpaceDeck.GameState.Execution
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using SpaceDeck.GameState.Minimum;

    public class PlayerChooseFromCardBrowser : PlayerChoice<List<CardInstance>>
    {
        public SpaceDeck.Tokenization.Evaluatables.CardsEvaluatableValue CardsToShow;
        public SpaceDeck.Tokenization.Evaluatables.INumericEvaluatableValue NumberOfCardsToChoose;

        public PlayerChooseFromCardBrowser(SpaceDeck.Tokenization.Evaluatables.CardsEvaluatableValue cardsToShow, SpaceDeck.Tokenization.Evaluatables.INumericEvaluatableValue numberOfCards)
        {
            this.CardsToShow = cardsToShow;
            this.NumberOfCardsToChoose = numberOfCards;
        }

        public override string DescribeChoice(IGameStateMutator mutator)
        {
            decimal numberOfCards = 0;
            if (!this.NumberOfCardsToChoose.TryEvaluate(mutator, out numberOfCards))
            {
                // TODO LOG
            }

            return $"Choose {numberOfCards} cards from {CardsToShow.Describe()}";
        }

        public override void SetChoice(IGameStateMutator mutator, List<CardInstance> result)
        {
            base.SetChoice(mutator, result);
            this.CardsToShow = new SpaceDeck.Tokenization.Evaluatables.ConstantCardsEvaluatableValue(result);
        }

        public override bool TryFinalizeWithoutPlayerInput(IGameStateMutator mutator)
        {
            if (!this.NumberOfCardsToChoose.TryEvaluate(mutator, out decimal numberOfCards))
            {
                return false;
            }

            // TODO: Determine if the number of picks remaining equals or exceeds the number of cards, then auto pick them all
            return false;
        }
    }
}