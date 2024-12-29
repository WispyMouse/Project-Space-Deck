namespace SpaceDeck.UX
{
    using SFDDCards;
    using SpaceDeck.Utility.Minimum;
    using SpaceDeck.Utility.Wellknown;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class RarityIndicator : MonoBehaviour
    {
        [SerializeReference]
        private Image IconShower;

        [SerializeReference]
        private Sprite StarterRarityIcon;

        [SerializeReference]
        private Sprite CommonRarityIcon;

        [SerializeReference]
        private Sprite UncommonRarityIcon;

        [SerializeReference]
        private Sprite RareRarityIcon;

        [SerializeReference]
        private Sprite GeneratedRarityIcon;


        [System.Obsolete("Transition to " + nameof(_SetFromRarity))]
        public void SetFromRarity(Card.KnownRarities rarity)
        {
            switch (rarity)
            {
                case Card.KnownRarities.Unknown:
                    this.IconShower.gameObject.SetActive(false);
                    break;
                case Card.KnownRarities.Starter:
                    this.IconShower.gameObject.SetActive(true);
                    this.IconShower.sprite = this.StarterRarityIcon;
                    break;
                case Card.KnownRarities.Common:
                    this.IconShower.gameObject.SetActive(true);
                    this.IconShower.sprite = this.CommonRarityIcon;
                    break;
                case Card.KnownRarities.Uncommon:
                    this.IconShower.gameObject.SetActive(true);
                    this.IconShower.sprite = this.UncommonRarityIcon;
                    break;
                case Card.KnownRarities.Rare:
                    this.IconShower.gameObject.SetActive(true);
                    this.IconShower.sprite = this.RareRarityIcon;
                    break;
                case Card.KnownRarities.Generated:
                    this.IconShower.gameObject.SetActive(true);
                    this.IconShower.sprite = this.GeneratedRarityIcon;
                    break;
            }
        }

        public void _SetFromRarity(LowercaseString rarity)
        {
            if (rarity == WellknownRarities.Starter)
            {
                this.IconShower.gameObject.SetActive(true);
                this.IconShower.sprite = this.StarterRarityIcon;

            }
            else if (rarity == WellknownRarities.Generated)
            {
                this.IconShower.gameObject.SetActive(true);
                this.IconShower.sprite = this.GeneratedRarityIcon;
            }
            else if (rarity == WellknownRarities.Common)
            {
                this.IconShower.gameObject.SetActive(true);
                this.IconShower.sprite = this.CommonRarityIcon;
            }
            else if (rarity == WellknownRarities.Uncommon)
            {
                this.IconShower.gameObject.SetActive(true);
                this.IconShower.sprite = this.UncommonRarityIcon;
            }
            else if (rarity == WellknownRarities.Rare)
            {
                this.IconShower.gameObject.SetActive(true);
                this.IconShower.sprite = this.RareRarityIcon;
            }
        }
    }
}