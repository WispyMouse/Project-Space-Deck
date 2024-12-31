namespace SpaceDeck.UX
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using SpaceDeck.GameState.Minimum;

    public class TargetableIndicator : MonoBehaviour
    {
        IChangeTarget Target { get; set; }
        Action<IChangeTarget> OnClickAction { get; set; } = null;
        Action<IChangeTarget> OnHoverAction { get; set; } = null;
        Action<IChangeTarget> OnHoverEndAction { get; set; } = null;

        public void SetFromTarget(IChangeTarget target, Action<IChangeTarget> onClickAction, Action<IChangeTarget> onHoverStart, Action<IChangeTarget> onHoverEnd)
        {
            this.Target = target;
            this.OnClickAction = onClickAction;
            this.OnHoverAction = onHoverStart;
            this.OnHoverEndAction = onHoverEnd;
        }

        public void OnMouseUpAsButton()
        {
            this.OnClickAction?.Invoke(this.Target);
        }

        public void OnMouseEnter()
        {
            this.OnHoverAction?.Invoke(this.Target);
        }

        public void OnMouseExit()
        {
            this.OnHoverEndAction?.Invoke(this.Target);
        }
    }
}