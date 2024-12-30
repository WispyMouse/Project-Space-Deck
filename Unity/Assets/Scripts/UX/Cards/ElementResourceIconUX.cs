namespace SpaceDeck.UX
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using UnityEngine;
    using UnityEngine.UI;
    using SpaceDeck.Utility.Wellknown;
    using SpaceDeck.UX.AssetLookup;

    public class ElementResourceIconUX : MonoBehaviour
    {
        [SerializeReference]
        private TMPro.TMP_Text NumbericIndicator;

        [SerializeReference]
        private Image SpriteSpot;

        public SpaceDeck.GameState.Minimum.Element RepresentingElement { get; private set; }

        public void SetFromElement(SpaceDeck.GameState.Minimum.Element representingElement, int count)
        {
            this.RepresentingElement = representingElement;
            this.NumbericIndicator.text = count.ToString();

            if (count > 0)
            {
                if (SpriteLookup.TryGetSprite(representingElement.Id, out Sprite sprite))
                {
                    this.SpriteSpot.sprite = sprite;
                }
            }
            else
            {
                if (SpriteLookup.TryGetSprite(representingElement.Id, out Sprite sprite, WellknownSprites.GreyScale))
                {
                    this.SpriteSpot.sprite = sprite;
                }
            }
        }
    }
}