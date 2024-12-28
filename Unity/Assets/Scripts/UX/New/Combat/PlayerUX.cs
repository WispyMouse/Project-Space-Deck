namespace SFDDCards.UX
{
    using SpaceDeck.GameState.Minimum;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class PlayerUX : MonoBehaviour, IAnimationPuppet
    {
        [Obsolete($"Should transition to {nameof(_RepresentedPlayer)}.")]
        public Player RepresentedPlayer { get; private set; }

        public Entity _RepresentedPlayer { get; private set; }

        public Transform OwnTransform => this.transform;
        public bool IsNotDestroyed => this != null && this?.gameObject != null;

        [Obsolete($"Should transition to {nameof(_SetFromPlayer)}.")]
        public void SetFromPlayer(Player toSet)
        {
            this.RepresentedPlayer = toSet;
            this.RepresentedPlayer.UXPositionalTransform = this.transform;
        }

        public void _SetFromPlayer(Entity toSet)
        {
            this._RepresentedPlayer = toSet;
            this.RepresentedPlayer.UXPositionalTransform = this.transform;
        }
    }
}