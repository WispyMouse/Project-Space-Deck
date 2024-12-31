namespace SpaceDeck.UX
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading;
    using UnityEngine;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.UX;
    using SpaceDeck.GameState.Execution;

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
        private Action<List<CardInstance>> SelectionFinishedAction { get; set; } = null;

        public int RemainingCardsToChoose { get; set; } = 0;

        private readonly List<CardInstance> ChosenCards = new List<CardInstance>();

        public void Awake()
        {
            this.Annihilate(false);
        }

        public void SetLabelText(string newText)
        {
            this.Label.text = newText;
        }

        public void SetFromCards(IEnumerable<CardInstance> cardsToShow)
        {
            this.gameObject.SetActive(true);
            this.Annihilate(false);
            this.CloseButton.SetActive(true);

            foreach (CardInstance curCard in cardsToShow)
            {
                RenderedCard newCard = Instantiate(this.RenderedCardPF, this.CardHolder);
                newCard.SetFromCard(curCard);
                newCard.OnClickAction = this.CardClicked;
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

            this.ChosenCards.Clear();
            this.RemainingCardsToChoose = 0;

            if (close)
            {
                this.Close();
            }
        }

        public void SetFromCardBrowserChoice(IGameStateMutator mutator, PlayerChooseFromCardBrowser toHandle, Action<List<CardInstance>> continuationAction)
        {
            if (!toHandle.NumberOfCardsToChoose.TryEvaluate(mutator, out decimal evaluatedNumberOfCards))
            {
                // TODO LOG
                return;
            }

            if (!toHandle.CardsToShow.TryEvaluate(mutator, out IReadOnlyList<CardInstance> evaluatedCards))
            {
                // TODO LOG
                return;
            }

            evaluatedNumberOfCards = Mathf.Min((int)evaluatedNumberOfCards, evaluatedCards.Count);

            if (evaluatedNumberOfCards == 0)
            {
                this.Close();
                continuationAction.Invoke(new List<CardInstance>());
                return;
            }

            this.SetFromCards(evaluatedCards);
            this.SetLabelText(toHandle.DescribeChoice(mutator));
            this.RemainingCardsToChoose = (int)evaluatedNumberOfCards;
            this.CloseButton.SetActive(false);
            SelectionFinishedAction = continuationAction;
        }

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
                List<CardInstance> chosenCards = new List<CardInstance>(this.ChosenCards);
                this.Close();
                this.SelectionFinishedAction?.Invoke(chosenCards);
            }
        }
    }
}
