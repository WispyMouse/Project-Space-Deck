namespace SpaceDeck.UX
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using SpaceDeck.Utility.Minimum;
    using SpaceDeck.Utility.Wellknown;

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