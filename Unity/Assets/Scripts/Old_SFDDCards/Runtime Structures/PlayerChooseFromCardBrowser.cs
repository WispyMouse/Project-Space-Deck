namespace SFDDCards
{
    using SFDDCards.Evaluation.Actual;
    using SFDDCards.Evaluation.Conceptual;
    using SFDDCards.ScriptingTokens;
    using SFDDCards.ScriptingTokens.EvaluatableValues;
    using SpaceDeck.GameState.Minimum;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using UnityEngine;

    public class PlayerChooseFromCardBrowser : PlayerChoice<List<CardInstance>>
    {
        [Obsolete("Transition to " + nameof(_CardsToShow))]
        public PromisedCardsEvaluatableValue CardsToShow;
        public SpaceDeck.Tokenization.Evaluatables.CardsEvaluatableValue _CardsToShow;
        [Obsolete("Transition to " + nameof(_NumberOfCardsToChoose))]
        public IEvaluatableValue<int> NumberOfCardsToChoose;
        public SpaceDeck.Tokenization.Evaluatables.INumericEvaluatableValue _NumberOfCardsToChoose;

        [Obsolete("Transition to constructor using SpaceDeck namespace.")]
        public PlayerChooseFromCardBrowser(PromisedCardsEvaluatableValue cardsToShow, IEvaluatableValue<int> numberOfCards)
        {
            this.CardsToShow = cardsToShow;
            this.NumberOfCardsToChoose = numberOfCards;
        }

        public PlayerChooseFromCardBrowser(SpaceDeck.Tokenization.Evaluatables.CardsEvaluatableValue cardsToShow, SpaceDeck.Tokenization.Evaluatables.INumericEvaluatableValue numberOfCards)
        {
            this._CardsToShow = cardsToShow;
            this._NumberOfCardsToChoose = numberOfCards;
        }

        [Obsolete("Should transition to " + nameof(_DescribeChoice))]
        public override string DescribeChoice(CampaignContext campaignContext, TokenEvaluatorBuilder currentEvaluator)
        {
            int numberOfCards = 0;
            if (!this.NumberOfCardsToChoose.TryEvaluateValue(campaignContext, currentEvaluator, out numberOfCards))
            {
                GlobalUpdateUX.LogTextEvent.Invoke($"Failed to parse number of cards for choice.", GlobalUpdateUX.LogType.RuntimeError);
            }

            return $"Choose {numberOfCards} cards from {CardsToShow.DescribeEvaluation()}";
        }

        public override string _DescribeChoice(IGameStateMutator mutator)
        {
            decimal numberOfCards = 0;
            if (!this._NumberOfCardsToChoose.TryEvaluate(mutator, out numberOfCards))
            {
                GlobalUpdateUX.LogTextEvent.Invoke($"Failed to parse number of cards for choice.", GlobalUpdateUX.LogType.RuntimeError);
            }

            return $"Choose {numberOfCards} cards from {_CardsToShow.Describe()}";
        }

        [Obsolete("Transition to the SetChoice with the new namespace")]
        public override void SetChoice(DeltaEntry toApplyTo, List<CardInstance> result)
        {
            throw new Exception("Cannot use this version of the choice anymore.");
        }

        public override void _SetChoice(IGameStateMutator mutator, List<CardInstance> result)
        {
            base._SetChoice(mutator, result);
            this._CardsToShow = new SpaceDeck.Tokenization.Evaluatables.ConstantCardsEvaluatableValue(result);
        }

        public override bool TryFinalizeWithoutPlayerInput(DeltaEntry toApplyTo)
        {
            if (!this.NumberOfCardsToChoose.TryEvaluateValue(toApplyTo.FromCampaign, toApplyTo.MadeFromBuilder, out int numberOfCards))
            {
                return false;
            }

            if (this.CardsToShow.InnerValue == null && this.CardsToShow.SampledPool != null && numberOfCards >= this.CardsToShow.SampledPool.RepresentingNumberOfCards(toApplyTo) && this.CardsToShow.SampledPool.TryEvaluateValue(toApplyTo.FromCampaign, toApplyTo.MadeFromBuilder, out List<Card> chosenCards))
            {
                this.CardsToShow.InnerValue = new SpecificCardsEvaluatableValue(chosenCards);
                // TODO: DISMANTLE
                // this.SetChoice(toApplyTo, chosenCards);
                return true;
            }

            if (this.CardsToShow.InnerValue != null && numberOfCards >= this.CardsToShow.InnerValue.RepresentingNumberOfCards(toApplyTo) && this.CardsToShow.InnerValue.TryEvaluateValue(toApplyTo.FromCampaign, toApplyTo.MadeFromBuilder, out chosenCards))
            {
                this.CardsToShow.InnerValue = new SpecificCardsEvaluatableValue(chosenCards);
                // TODO: DISMANTLE
                // this.SetChoice(toApplyTo, chosenCards);
                return true;
            }

            return false;
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