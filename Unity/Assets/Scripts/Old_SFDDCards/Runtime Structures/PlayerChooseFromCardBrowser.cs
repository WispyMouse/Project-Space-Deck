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

    public class PlayerChooseFromCardBrowser : PlayerChoice<List<Card>>
    {
        [Obsolete("Transition to " + nameof(_CardsToShow))]
        public PromisedCardsEvaluatableValue CardsToShow;
        public SpaceDeck.Tokenization.Evaluatables.IEvaluatableValue<List<CardInstance>> _CardsToShow;
        [Obsolete("Transition to " + nameof(_NumberOfCardsToChoose))]
        public IEvaluatableValue<int> NumberOfCardsToChoose;
        public SpaceDeck.Tokenization.Evaluatables.INumericEvaluatableValue _NumberOfCardsToChoose;

        public PlayerChooseFromCardBrowser(PromisedCardsEvaluatableValue cardsToShow, IEvaluatableValue<int> numberOfCards)
        {
            this.CardsToShow = cardsToShow;
            this.NumberOfCardsToChoose = numberOfCards;
        }

        public PlayerChooseFromCardBrowser(SpaceDeck.Tokenization.Evaluatables.IEvaluatableValue<List<CardInstance>> cardsToShow, SpaceDeck.Tokenization.Evaluatables.INumericEvaluatableValue numberOfCards)
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

        public override void SetChoice(DeltaEntry toApplyTo, List<Card> result)
        {
            base.SetChoice(toApplyTo, result);
            this.CardsToShow.InnerValue = new SpecificCardsEvaluatableValue(result);
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
                this.SetChoice(toApplyTo, chosenCards);
                return true;
            }

            if (this.CardsToShow.InnerValue != null && numberOfCards >= this.CardsToShow.InnerValue.RepresentingNumberOfCards(toApplyTo) && this.CardsToShow.InnerValue.TryEvaluateValue(toApplyTo.FromCampaign, toApplyTo.MadeFromBuilder, out chosenCards))
            {
                this.CardsToShow.InnerValue = new SpecificCardsEvaluatableValue(chosenCards);
                this.SetChoice(toApplyTo, chosenCards);
                return true;
            }

            return false;
        }
    }
}