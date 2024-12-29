namespace SpaceDeck.UX
{
    using SFDDCards;
    using SFDDCards.UX;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.UX.AssetLookup;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public class RewardArtifactUX : MonoBehaviour, IMouseHoverListener
    {
        [Obsolete("Should transition to " + nameof(_RepresentedArtifact))]
        public StatusEffect RepresentedArtifact { get; set; }
        public SpaceDeck.GameState.Minimum.AppliedStatusEffect _RepresentedArtifact { get; private set; }

        [SerializeReference]
        private Image ImageRepresentation;

        [SerializeReference]
        private TMPro.TMP_Text Name;

        [SerializeReference]
        private TMPro.TMP_Text Label;

        [SerializeReference]
        public LayoutElement OwnLayoutElement;

        public virtual bool ShouldShowBase { get; } = false;

        private Action<RewardArtifactUX> OnClicked {get; set;} 

        [Obsolete("Transition to " + nameof(SetFromArtifact))]
        public void SetFromArtifact(StatusEffect artifact, Action<RewardArtifactUX> onClick, int amount)
        {
            this.RepresentedArtifact = artifact;

            this.ImageRepresentation.sprite = artifact.Sprite;
            this.Name.text = artifact.Name;
            this.Label.text = (amount != 0 ? $"x{amount}" : "") + artifact.DescribeStatusEffect().BreakDescriptionsIntoString();
            this.OnClicked = onClick;
        }

        public void SetFromArtifact(SpaceDeck.GameState.Minimum.AppliedStatusEffect artifact, Action<RewardArtifactUX> onClick, int amount)
        {
            this._RepresentedArtifact = artifact;

            if (SpriteLookup.TryGetSprite(artifact.Id, out Sprite sprite))
            {
                this.ImageRepresentation.sprite = sprite;
            }

            this.Name.text = artifact.Name;
            this.Label.text = (amount != 0 ? $"x{amount}" : "") + artifact.Describe();
            this.OnClicked = onClick;
        }

        public void Clicked()
        {
            this.gameObject.SetActive(false);
            this.OnClicked.Invoke(this);
        }

        public virtual void MouseEnterStartHover()
        {
            MouseHoverShowerController.MouseStartHoveredEvent.Invoke(this);
        }

        public virtual void MouseExitStopHover()
        {
            MouseHoverShowerController.MouseEndHoveredEvent.Invoke(this);
        }

        public Transform GetTransform()
        {
            return this.transform;
        }

        public bool TryGetCard(out Card toShow)
        {
            toShow = null;
            return false;
        }
        public bool _TryGetCard(out CardInstance toShow)
        {
            toShow = null;
            return false;
        }

        public bool TryGetStatusEffect(out IStatusEffect toShow)
        {
            toShow = this.RepresentedArtifact;
            return true;
        }

        public bool _TryGetStatusEffect(out SpaceDeck.GameState.Minimum.AppliedStatusEffect toShow)
        {
            toShow = this._RepresentedArtifact;
            return true;
        }

        public void UnHoverOnDisable()
        {
            MouseHoverShowerController.MouseEndHoveredEvent.Invoke(this);
        }

        private void OnDisable()
        {
            this.UnHoverOnDisable();
        }

        private void OnDestroy()
        {
            this.UnHoverOnDisable();
        }

    }
}