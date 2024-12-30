namespace SpaceDeck.UX
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using SpaceDeck.GameState.Minimum;

    public interface IMouseHoverListener
    {
        public Transform GetTransform();
        public bool _TryGetCard(out CardInstance toShow);
        public bool _TryGetStatusEffect(out SpaceDeck.GameState.Minimum.AppliedStatusEffect toShow);
        public bool ShouldShowBase { get; }
        public void UnHoverOnDisable();
    }
}