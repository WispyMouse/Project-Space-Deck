namespace SpaceDeck.UX
{
    using SFDDCards;
    using SFDDCards.Evaluation.Actual;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Utility.Wellknown;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class EnemyUX : MonoBehaviour, IAnimationPuppet
    {
        [Obsolete("Should transition to " + nameof(_RepresentedEnemy))]
        public Enemy RepresentedEnemy { get; private set; } = null;
        public Entity _RepresentedEnemy { get; private set; } = null;

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

        public void _SetFromEnemy(Entity toSet, CentralGameStateController centralGameStateController)
        {
            this._RepresentedEnemy = toSet;

            this.RepresentedEnemy.UXPositionalTransform = this.transform;

            this.ClearEffectText();
            this.OwnStatusEffectHolder.Annihilate();

            this._UpdateUX(centralGameStateController);
        }

        public void _UpdateUX(CentralGameStateController centralGameStateController)
        {
            this.Name.text = this._RepresentedEnemy.Qualities.GetStringQuality(WellknownQualities.Name);
            this.Health.text = $"{this._RepresentedEnemy.Qualities.GetNumericQuality(WellknownQualities.Health)} / {this._RepresentedEnemy.Qualities.GetNumericQuality(WellknownQualities.MaximumHealth)}";
            this._UpdateIntent(centralGameStateController);

            this.OwnStatusEffectHolder._SetStatusEffects(this._RepresentedEnemy.AppliedStatusEffects.Values);
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

        [Obsolete("Should transition to " + nameof(_UpdateIntent))]
        void UpdateIntent(CentralGameStateController centralGameStateController)
        {
            string description = "";

            if (this.RepresentedEnemy.Intent != null)
            {
                description = EffectDescriberDatabase.DescribeRealizedEffect(
                    this.RepresentedEnemy.Intent,
                    centralGameStateController.CurrentCampaignContext,
                    this.RepresentedEnemy,
                    centralGameStateController.CurrentCampaignContext.CurrentCombatContext.CombatPlayer
                    );
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

        void _UpdateIntent(CentralGameStateController centralGameStateController)
        {
            string description = "";

            if (this._RepresentedEnemy.CurrentIntent != null)
            {
                description = this._RepresentedEnemy.CurrentIntent.Describe();
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