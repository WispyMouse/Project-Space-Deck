namespace SpaceDeck.UX
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Utility.Wellknown;
    using SpaceDeck.UX.AssetLookup;

    public class RenderedCard : MonoBehaviour
    {
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