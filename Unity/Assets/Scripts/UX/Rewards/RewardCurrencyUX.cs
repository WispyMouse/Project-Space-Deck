namespace SpaceDeck.UX
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Models.Instances;
    using SpaceDeck.UX.AssetLookup;

    public class RewardCurrencyUX : MonoBehaviour, IMouseHoverListener
    {
        [SerializeReference]
        private Image ImageRepresentation;

        [SerializeReference]
        private TMPro.TMP_Text Name;

        [SerializeReference]
        private TMPro.TMP_Text Label;

        [SerializeReference]
        public LayoutElement OwnLayoutElement;

        public Currency RepresentedCurrency;
        public int RewardAmount;

        private Action<RewardCurrencyUX> OnClicked { get; set; }

        public bool ShouldShowBase => true;

        public void SetFromCurrency(Currency basedOn, Action<RewardCurrencyUX> onClick, int amount)
        {
            this.RepresentedCurrency = basedOn;

            if (SpriteLookup.TryGetSprite(basedOn.Id, out Sprite currencySprite))
            {
                this.ImageRepresentation.sprite = currencySprite;
            }

            this.Name.text = basedOn.Name;
            this.RewardAmount = amount;
            this.Label.text = $"x{RewardAmount.ToString()} {SpriteLookup.GetNameAndMaybeIcon(basedOn)}";
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

        public bool TryGetStatusEffect(out GameState.Minimum.AppliedStatusEffect toShow)
        {
            toShow = null;
            return false;
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