namespace SFDDCards.UX
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.UX;
    using SpaceDeck.UX.AssetLookup;

    public class StatusEffectUX : MonoBehaviour, IMouseHoverListener
    {
        [Obsolete("Should transition to " + nameof(_RepresentsEffect))]
        public SFDDCards.AppliedStatusEffect RepresentsEffect;
        public AppliedStatusEffect _RepresentsEffect;

        [SerializeReference]
        private TMPro.TMP_Text StackText;

        [SerializeReference]
        private Image Renderer;

        [Obsolete("Should transition to " + nameof(_OnStatusEffectPressed))]
        private Action<SFDDCards.AppliedStatusEffect> OnStatusEffectPressed;
        private Action<AppliedStatusEffect> _OnStatusEffectPressed;

        public virtual bool ShouldShowBase { get; } = true;

        public Transform GetTransform()
        {
            return this.GetComponent<RectTransform>();
        }

        [Obsolete("Should transition to " + nameof(_SetFromEffect))]
        public void SetFromEffect(SFDDCards.AppliedStatusEffect toSet, Action<SFDDCards.AppliedStatusEffect> onClickEvent = null)
        {
            this.RepresentsEffect = toSet;
            this.Renderer.sprite = toSet.BasedOnStatusEffect.Sprite;
            this.OnStatusEffectPressed = onClickEvent;
        }

        public void _SetFromEffect(AppliedStatusEffect toSet, Action<AppliedStatusEffect> onClickEvent = null)
        {
            this._RepresentsEffect = toSet;
            this._OnStatusEffectPressed = onClickEvent;

            if (SpriteLookup.TryGetSprite(toSet.Id, out Sprite displaySprite))
            {
                this.Renderer.sprite = displaySprite;
            }
        }

        public void SetStacks(int toStack)
        {
            if (toStack <= 1)
            {
                this.StackText.gameObject.SetActive(false);
                return;
            }

            this.StackText.gameObject.SetActive(true);
            this.StackText.text = toStack.ToString();
        }

        public bool TryGetCard(out Card toShow)
        {
            toShow = null;
            return false;
        }

        public bool TryGetStatusEffect(out IStatusEffect toShow)
        {
            toShow = this.RepresentsEffect;
            return true;
        }

        public virtual void MouseEnterStartHover()
        {
            MouseHoverShowerController.MouseStartHoveredEvent.Invoke(this);
        }

        public virtual void MouseExitStopHover()
        {
            MouseHoverShowerController.MouseEndHoveredEvent.Invoke(this);
        }

        private void OnDisable()
        {
            this.UnHoverOnDisable();
        }

        public void Clicked()
        {
            this.OnStatusEffectPressed?.Invoke(this.RepresentsEffect);
        }

        public void UnHoverOnDisable()
        {
            MouseHoverShowerController.MouseEndHoveredEvent.Invoke(this);
        }
    }
}