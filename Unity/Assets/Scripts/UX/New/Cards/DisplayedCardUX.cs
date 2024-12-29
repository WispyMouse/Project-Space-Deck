namespace SpaceDeck.UX
{
    using SFDDCards;
    using SFDDCards.UX;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.UX;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class DisplayedCardUX : MonoBehaviour, IMouseHoverListener
    {
        [Obsolete("Transition to " + nameof(_RepresentedCard))]
        public Card RepresentedCard { get; private set; } = null;
        public CardInstance _RepresentedCard { get; private set; } = null;
        
        [SerializeReference]
        private RenderedCard RenderedCard;

        Action<DisplayedCardUX> cardSelectedAction { get; set; } = null;

        public virtual bool ShouldShowBase { get; } = true;

        public void Awake()
        {
            this.DisableSelectionGlow();
        }

        public virtual void MouseEnterStartHover()
        {
            MouseHoverShowerPanel.CurrentContext = null;
            MouseHoverShowerController.MouseStartHoveredEvent.Invoke(this);
        }

        public virtual void MouseExitStopHover()
        {
            MouseHoverShowerController.MouseEndHoveredEvent.Invoke(this);
        }

        public virtual void Clicked()
        {
            this.cardSelectedAction.Invoke(this);
        }

        public virtual void EnableSelectionGlow()
        {
        }

        public virtual void DisableSelectionGlow()
        {
        }

        [Obsolete("Transition to " + nameof(_SetFromCard))]
        public void SetFromCard(Card toSet, Action<DisplayedCardUX> inCardSelectedAction = null, ReactionWindowContext? reactionWindowContext = null)
        {
            this.RepresentedCard = toSet;
            this.cardSelectedAction = inCardSelectedAction;

            this.RenderedCard.SetFromCard(toSet, reactionWindowContext);
        }

        public void _SetFromCard(CardInstance toSet, Action<DisplayedCardUX> inCardSelectedAction = null, ReactionWindowContext? reactionWindowContext = null)
        {
            this._RepresentedCard = toSet;
            this.cardSelectedAction = inCardSelectedAction;

            this.RenderedCard._SetFromCard(toSet, reactionWindowContext);
        }

        public Transform GetTransform()
        {
            return this.transform;
        }

        public bool TryGetCard(out Card toShow)
        {
            toShow = this.RenderedCard?.RepresentedCard;
            return toShow != null;
        }

        public bool _TryGetCard(out CardInstance toShow)
        {
            toShow = this.RenderedCard?._RepresentedCard;
            return toShow != null;
        }

        private void OnDisable()
        {
            this.UnHoverOnDisable();
        }

        private void OnDestroy()
        {
            this.UnHoverOnDisable();
        }

        public bool TryGetStatusEffect(out IStatusEffect toShow)
        {
            toShow = null;
            return false;
        }

        public bool _TryGetStatusEffect(out SpaceDeck.GameState.Minimum.AppliedStatusEffect toShow)
        {
            toShow = null;
            return false;
        }

        public void UnHoverOnDisable()
        {
            MouseHoverShowerController.MouseEndHoveredEvent.Invoke(this);
        }

    }
}