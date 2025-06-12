namespace SpaceDeck.Creator
{
    using System.Collections;
    using System.Collections.Generic;
    using SpaceDeck.Models.Databases;
    using SpaceDeck.Models.Prototypes;
    using SpaceDeck.UX;
    using UnityEngine;

    public class CardsSelectorPanel : MonoBehaviour
    {
        [SerializeReference]
        private CardSelectionButton CardSelectionButtonPF;

        [SerializeReference]
        private Transform CardSelectionButtonParent;

        [SerializeReference]
        private CardCreatorPanel CardCreatorPanel;

        void Start()
        {
            DefaultLoader.CallWhenLoaded(UpdateCardList);
        }

        public void UpdateCardList()
        {
            for (int ii = CardSelectionButtonParent.childCount - 1; ii >= 0; ii--)
            {
                Destroy(CardSelectionButtonParent.GetChild(ii).gameObject);
            }

            IEnumerable<CardPrototype> prototypes = CardDatabase.AllPrototypes;

            foreach (CardPrototype prototype in prototypes)
            {
                CardSelectionButton newButton = Instantiate(this.CardSelectionButtonPF, this.CardSelectionButtonParent);
                newButton.SetCard(prototype, CardCreatorPanel.ShowCard);
            }
        }
    }
}