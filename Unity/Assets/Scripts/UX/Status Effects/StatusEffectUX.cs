namespace SpaceDeck.UX
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
        public AppliedStatusEffect RepresentsEffect;

        [SerializeReference]
        private TMPro.TMP_Text StackText;

        [SerializeReference]
        private Image Renderer;

        private Action<AppliedStatusEffect> OnStatusEffectPressed;

        public virtual bool ShouldShowBase { get; } = true;

        public Transform GetTransform()
        {
            return this.GetComponent<RectTransform>();
        }

        public void SetFromEffect(AppliedStatusEffect toSet, Action<AppliedStatusEffect> onClickEvent = null)
        {
            this.RepresentsEffect = toSet;
            this.OnStatusEffectPressed = onClickEvent;

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

        public bool TryGetCard(out CardInstance toShow)
        {
            toShow = null;
            return false;
        }

        public bool TryGetStatusEffect(out AppliedStatusEffect toShow)
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