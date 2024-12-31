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
        public Entity RepresentedPlayer { get; private set; }

        public Transform OwnTransform => this.transform;
        public bool IsNotDestroyed => this != null && this?.gameObject != null;

        public void SetFromPlayer(Entity toSet)
        {
            this.RepresentedPlayer = toSet;
        }
    }
}