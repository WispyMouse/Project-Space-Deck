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
        public SpaceDeck.Tokenization.Evaluatables.CardsEvaluatableValue _CardsToShow;
        public SpaceDeck.Tokenization.Evaluatables.INumericEvaluatableValue _NumberOfCardsToChoose;

        public PlayerChooseFromCardBrowser(SpaceDeck.Tokenization.Evaluatables.CardsEvaluatableValue cardsToShow, SpaceDeck.Tokenization.Evaluatables.INumericEvaluatableValue numberOfCards)
        {
            this._CardsToShow = cardsToShow;
            this._NumberOfCardsToChoose = numberOfCards;
        }

        public override string _DescribeChoice(IGameStateMutator mutator)
        {
            decimal numberOfCards = 0;
            if (!this._NumberOfCardsToChoose.TryEvaluate(mutator, out numberOfCards))
            {
                // TODO LOG
            }

            return $"Choose {numberOfCards} cards from {_CardsToShow.Describe()}";
        }

        public override void _SetChoice(IGameStateMutator mutator, List<CardInstance> result)
        {
            base._SetChoice(mutator, result);
            this._CardsToShow = new SpaceDeck.Tokenization.Evaluatables.ConstantCardsEvaluatableValue(result);
        }

        public override bool _TryFinalizeWithoutPlayerInput(IGameStateMutator mutator)
        {
            if (!this._NumberOfCardsToChoose.TryEvaluate(mutator, out decimal numberOfCards))
            {
                return false;
            }

            // TODO: Determine if the number of picks remaining equals or exceeds the number of cards, then auto pick them all
            return false;
        }
    }
}