namespace SpaceDeck.Creator
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using SpaceDeck.Models.Databases;
    using SpaceDeck.Models.Prototypes;
    using SpaceDeck.UX;
    using TMPro;
    using UnityEngine;

    public class CardSelectionButton : MonoBehaviour
    {
        public CardPrototype RepresentedCard { get; private set; }
        private Action<CardPrototype> OnClickAction { get; set; }

        [SerializeReference]
        private TMP_Text TextLabel;

        public void SetCard(CardPrototype cardPrototype, Action<CardPrototype> onClick)
        {
            this.OnClickAction = onClick;
            this.RepresentedCard = cardPrototype;

            this.TextLabel.text = cardPrototype.Id;
        }

        public void OnClick()
        {
            this.OnClickAction?.Invoke(this.RepresentedCard);
        }
    }
}