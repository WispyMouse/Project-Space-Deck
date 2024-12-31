namespace SpaceDeck.UX
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Utility.Wellknown;

    public class EnemyUX : MonoBehaviour, IAnimationPuppet
    {
        public Entity RepresentedEnemy { get; private set; } = null;

        public Transform OwnTransform => this.transform;

        public bool IsNotDestroyed => this != null && this?.gameObject != null;

        [SerializeReference]
        private TMPro.TMP_Text Name;

        [SerializeReference]
        private TMPro.TMP_Text Health;

        [SerializeReference]
        private TMPro.TMP_Text EffectText;

        [SerializeReference]
        private EnemyStatusEffectUXHolder OwnStatusEffectHolder;

        public void SetFromEnemy(Entity toSet, CentralGameStateController centralGameStateController)
        {
            this.RepresentedEnemy = toSet;

            this.ClearEffectText();
            this.OwnStatusEffectHolder.Annihilate();

            this.UpdateUX(centralGameStateController);
        }

        public void UpdateUX(CentralGameStateController centralGameStateController)
        {
            this.Name.text = this.RepresentedEnemy.Qualities.GetStringQuality(WellknownQualities.Name);
            this.Health.text = $"{this.RepresentedEnemy.Qualities.GetNumericQuality(WellknownQualities.Health)} / {this.RepresentedEnemy.Qualities.GetNumericQuality(WellknownQualities.MaximumHealth)}";
            this.UpdateIntent(centralGameStateController);

            this.OwnStatusEffectHolder.SetStatusEffects(this.RepresentedEnemy.AppliedStatusEffects.Values);
        }

        public void SetEffectText(string toText)
        {
            this.EffectText.text = toText;
            this.EffectText.gameObject.SetActive(true);
        }

        public void ClearEffectText()
        {
            this.EffectText.gameObject.SetActive(false);
        }

        void UpdateIntent(CentralGameStateController centralGameStateController)
        {
            string description = "";

            if (this.RepresentedEnemy.CurrentIntent != null)
            {
                description = this.RepresentedEnemy.CurrentIntent.Describe();
            }

            if (!string.IsNullOrEmpty(description))
            {
                this.SetEffectText(description);
            }
            else
            {
                this.ClearEffectText();
            }
        }
    }
}