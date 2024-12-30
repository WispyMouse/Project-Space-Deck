namespace SpaceDeck.UX
{
    using SFDDCards;
    using SFDDCards.UX;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Utility.Wellknown;
    using SpaceDeck.UX.AssetLookup;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public class RenderedCard : MonoBehaviour
    {
        [Obsolete("Should transition to " + nameof(_RepresentedCard))]
        public Card RepresentedCard;
        public CardInstance _RepresentedCard;

        [SerializeReference]
        private TMPro.TMP_Text NameText;

        [SerializeReference]
        private TMPro.TMP_Text EffectText;

        [SerializeReference]
        private Image CardImage;

        [SerializeReference]
        private ElementResourceIconUX ElementResourceIconUXPF;

        [SerializeReference]
        private Transform ElementResourceIconHolder;

        [SerializeReference]
        private RarityIndicator MyRarityIndicator;

        public Action<RenderedCard> OnClickAction { get; set; } = null;

        [Obsolete("Transition to " + nameof(_SetFromCard))]
        public void SetFromCard(Card representedCard, ReactionWindowContext? reactionWindowContext = null)
        {
            this.Annihilate();

            this.RepresentedCard = representedCard;

            this.NameText.text = representedCard.Name;
            this.CardImage.sprite = representedCard.Sprite;
            this.EffectText.text = representedCard.GetDescription(reactionWindowContext).BreakDescriptionsIntoString();

            foreach (SFDDCards.Element curElement in ElementDatabase.ElementData.Values)
            {
                ElementResourceIconUX icon = Instantiate(this.ElementResourceIconUXPF, this.ElementResourceIconHolder);
                // TODO DISSASEMBLE
                // icon.SetFromElement(curElement, representedCard.GetElementGain(curElement));
            }

            this.MyRarityIndicator.SetFromRarity(representedCard.Rarity);
        }

        public void _SetFromCard(CardInstance representedCard)
        {
            this.Annihilate();

            this._RepresentedCard = representedCard;

            this.NameText.text = representedCard.Name;
            this.EffectText.text = representedCard.Describe();

            if (SpriteLookup.TryGetSprite(representedCard.Id, out Sprite cardArt))
            {
                this.CardImage.sprite = cardArt;
            }

            foreach (SpaceDeck.GameState.Minimum.Element curElement in SpaceDeck.Models.Databases.ElementDatabase.ElementData.Values)
            {
                ElementResourceIconUX icon = Instantiate(this.ElementResourceIconUXPF, this.ElementResourceIconHolder);
                icon.SetFromElement(curElement, SpaceDeck.Models.Databases.ElementDatabase.GetElementGain(curElement, representedCard));
            }

            this.MyRarityIndicator._SetFromRarity(representedCard.Qualities.GetStringQuality(WellknownQualities.Rarity));
        }

        public void Annihilate()
        {
            this.NameText.text = "";
            this.CardImage.sprite = null;
            this.EffectText.text = "";
            this.RepresentedCard = null;
            this._RepresentedCard = null;

            for (int ii = this.ElementResourceIconHolder.childCount - 1; ii >= 0; ii--)
            {
                Destroy(this.ElementResourceIconHolder.GetChild(ii).gameObject);
            }
        }

        public void Clicked()
        {
            this.OnClickAction?.Invoke(this);
        }
    }
}