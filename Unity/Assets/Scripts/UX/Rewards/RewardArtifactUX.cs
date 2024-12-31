namespace SpaceDeck.UX
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.UX.AssetLookup;

    public class RewardArtifactUX : MonoBehaviour, IMouseHoverListener
    {
        public SpaceDeck.GameState.Minimum.AppliedStatusEffect RepresentedArtifact { get; private set; }

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

        public void SetFromArtifact(SpaceDeck.GameState.Minimum.AppliedStatusEffect artifact, Action<RewardArtifactUX> onClick, int amount)
        {
            this.RepresentedArtifact = artifact;

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

        public bool TryGetCard(out CardInstance toShow)
        {
            toShow = null;
            return false;
        }


        public bool TryGetStatusEffect(out SpaceDeck.GameState.Minimum.AppliedStatusEffect toShow)
        {
            toShow = this.RepresentedArtifact;
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