namespace SpaceDeck.UX
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using SpaceDeck.GameState.Minimum;

    public class PlayerUX : MonoBehaviour, IAnimationPuppet
    {
        public Entity _RepresentedPlayer { get; private set; }

        public Transform OwnTransform => this.transform;
        public bool IsNotDestroyed => this != null && this?.gameObject != null;

        public void _SetFromPlayer(Entity toSet)
        {
            this._RepresentedPlayer = toSet;
        }
    }
}