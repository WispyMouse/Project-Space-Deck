namespace SpaceDeck.UX
{
    using SFDDCards;
    using SFDDCards.Evaluation.Actual;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.UX;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading;
    using UnityEngine;


    public class CardBrowser : MonoBehaviour
    {
        [SerializeReference]
        private Transform CardHolder;

        [SerializeReference]
        private RenderedCard RenderedCardPF;

        [SerializeReference]
        private TMPro.TMP_Text Label;

        [SerializeField]
        private GameObject CloseButton;

        [Obsolete("Transition to " + nameof(_SelectionFinishedAction))]
        private Action<List<Card>> SelectionFinishedAction { get; set; } = null;
        private Action<List<CardInstance>> _SelectionFinishedAction { get; set; } = null;

        public int RemainingCardsToChoose { get; set; } = 0;

        [Obsolete("Transition to " + nameof(_ChosenCards))]
        private List<Card> ChosenCards { get; set; } = new List<Card>();
        private readonly List<CardInstance> _ChosenCards = new List<CardInstance>();

        public void Awake()
        {
            this.Annihilate(false);
        }

        public void SetLabelText(string newText)
        {
            this.Label.text = newText;
        }

        [Obsolete("Transition to " + nameof(_SetFromCards))]
        public void SetFromCards(IEnumerable<Card> cardsToShow)
        {
            this.gameObject.SetActive(true);
            this.Annihilate(false);
            this.CloseButton.SetActive(true);

            foreach (Card curCard in cardsToShow)
            {
                RenderedCard newCard = Instantiate(this.RenderedCardPF, this.CardHolder);
                newCard.SetFromCard(curCard, null);
                newCard.OnClickAction = this.CardClicked;
            }
        }

        public void _SetFromCards(IEnumerable<CardInstance> cardsToShow)
        {
            this.gameObject.SetActive(true);
            this.Annihilate(false);
            this.CloseButton.SetActive(true);

            foreach (CardInstance curCard in cardsToShow)
            {
                RenderedCard newCard = Instantiate(this.RenderedCardPF, this.CardHolder);
                newCard._SetFromCard(curCard);
                newCard.OnClickAction = this._CardClicked;
            }
        }

        public void Close()
        {
            this.Annihilate(false);
            this.gameObject.SetActive(false);
        }

        public void Annihilate(bool close = true)
        {
            for (int ii = this.CardHolder.childCount - 1; ii >= 0; ii--)
            {
                Destroy(this.CardHolder.GetChild(ii).gameObject);
            }

            this.ChosenCards = new List<Card>();
            this._ChosenCards.Clear();
            this.RemainingCardsToChoose = 0;

            if (close)
            {
                this.Close();
            }
        }

        [Obsolete("Transition to " + nameof(_SetFromCardBrowserChoice))]
        public void SetFromCardBrowserChoice(DeltaEntry fromDelta, PlayerChooseFromCardBrowser toHandle, Action<List<Card>> continuationAction)
        {
            if (!toHandle.NumberOfCardsToChoose.TryEvaluateValue(fromDelta.FromCampaign, fromDelta.MadeFromBuilder, out int evaluatedNumberOfCards))
            {
                GlobalUpdateUX.LogTextEvent.Invoke($"Failed to evaluate number of cards for browser choice.", GlobalUpdateUX.LogType.RuntimeError);
                return;
            }

            if (!toHandle.CardsToShow.TryEvaluateValue(fromDelta.FromCampaign, fromDelta.MadeFromBuilder, out List<Card> evaluatedCards))
            {
                GlobalUpdateUX.LogTextEvent.Invoke($"Failed to evaluate card zone for browser choice.", GlobalUpdateUX.LogType.RuntimeError);
                return;
            }

            evaluatedNumberOfCards = Mathf.Min(evaluatedNumberOfCards, evaluatedCards.Count);

            if (evaluatedNumberOfCards == 0)
            {
                this.Close();
                continuationAction.Invoke(new List<Card>());
                return;
            }

            this.SetFromCards(evaluatedCards);
            this.SetLabelText(toHandle.DescribeChoice(fromDelta.FromCampaign, fromDelta.MadeFromBuilder));
            this.RemainingCardsToChoose = evaluatedNumberOfCards;
            this.CloseButton.SetActive(false);
            SelectionFinishedAction = continuationAction;
        }

        public void _SetFromCardBrowserChoice(IGameStateMutator mutator, PlayerChooseFromCardBrowser toHandle, Action<List<CardInstance>> continuationAction)
        {
            if (!toHandle._NumberOfCardsToChoose.TryEvaluate(mutator, out decimal evaluatedNumberOfCards))
            {
                GlobalUpdateUX.LogTextEvent.Invoke($"Failed to evaluate number of cards for browser choice.", GlobalUpdateUX.LogType.RuntimeError);
                return;
            }

            if (!toHandle._CardsToShow.TryEvaluate(mutator, out IReadOnlyList<CardInstance> evaluatedCards))
            {
                GlobalUpdateUX.LogTextEvent.Invoke($"Failed to evaluate card zone for browser choice.", GlobalUpdateUX.LogType.RuntimeError);
                return;
            }

            evaluatedNumberOfCards = Mathf.Min((int)evaluatedNumberOfCards, evaluatedCards.Count);

            if (evaluatedNumberOfCards == 0)
            {
                this.Close();
                continuationAction.Invoke(new List<CardInstance>());
                return;
            }

            this._SetFromCards(evaluatedCards);
            this.SetLabelText(toHandle._DescribeChoice(mutator));
            this.RemainingCardsToChoose = (int)evaluatedNumberOfCards;
            this.CloseButton.SetActive(false);
            _SelectionFinishedAction = continuationAction;
        }

        [Obsolete("Transition to " + nameof(_CardClicked))]
        public void CardClicked(RenderedCard chosenCard)
        {
            if (this.RemainingCardsToChoose == 0)
            {
                return;
            }

            this.ChosenCards.Add(chosenCard.RepresentedCard);
            Destroy(chosenCard.gameObject);

            this.RemainingCardsToChoose--;

            if (this.RemainingCardsToChoose == 0)
            {
                List<Card> chosenCards = new List<Card>(this.ChosenCards);
                this.Close();
                this.SelectionFinishedAction?.Invoke(chosenCards);
            }
        }

        public void _CardClicked(RenderedCard chosenCard)
        {
            if (this.RemainingCardsToChoose == 0)
            {
                return;
            }

            this._ChosenCards.Add(chosenCard._RepresentedCard);
            Destroy(chosenCard.gameObject);

            this.RemainingCardsToChoose--;

            if (this.RemainingCardsToChoose == 0)
            {
                List<CardInstance> chosenCards = new List<CardInstance>(this._ChosenCards);
                this.Close();
                this._SelectionFinishedAction?.Invoke(chosenCards);
            }
        }
    }
}
