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
        private Action<List<CardInstance>> _SelectionFinishedAction { get; set; } = null;

        public int RemainingCardsToChoose { get; set; } = 0;

        private readonly List<CardInstance> _ChosenCards = new List<CardInstance>();

        public void Awake()
        {
            this.Annihilate(false);
        }

        public void SetLabelText(string newText)
        {
            this.Label.text = newText;
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

            this._ChosenCards.Clear();
            this.RemainingCardsToChoose = 0;

            if (close)
            {
                this.Close();
            }
        }

        public void _SetFromCardBrowserChoice(IGameStateMutator mutator, PlayerChooseFromCardBrowser toHandle, Action<List<CardInstance>> continuationAction)
        {
            if (!toHandle._NumberOfCardsToChoose.TryEvaluate(mutator, out decimal evaluatedNumberOfCards))
            {
                // TODO LOG
                return;
            }

            if (!toHandle._CardsToShow.TryEvaluate(mutator, out IReadOnlyList<CardInstance> evaluatedCards))
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

            this._SetFromCards(evaluatedCards);
            this.SetLabelText(toHandle._DescribeChoice(mutator));
            this.RemainingCardsToChoose = (int)evaluatedNumberOfCards;
            this.CloseButton.SetActive(false);
            _SelectionFinishedAction = continuationAction;
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
