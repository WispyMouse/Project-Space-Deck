namespace SFDDCards.UX
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;

    public interface IMouseHoverListener
    {
        public Transform GetTransform();
        public bool TryGetCard(out Card toShow);
        [Obsolete("Transition to " + nameof(_TryGetStatusEffect))]
        public bool TryGetStatusEffect(out IStatusEffect toShow);
        public bool _TryGetStatusEffect(out SpaceDeck.GameState.Minimum.AppliedStatusEffect toShow);
        public bool ShouldShowBase { get; }
        public void UnHoverOnDisable();
    }
}