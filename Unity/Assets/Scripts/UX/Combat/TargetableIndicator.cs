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
        IChangeTarget _Target { get; set; }
        Action<IChangeTarget> _OnClickAction { get; set; } = null;
        Action<IChangeTarget> _OnHoverAction { get; set; } = null;
        Action<IChangeTarget> _OnHoverEndAction { get; set; } = null;

        public void _SetFromTarget(IChangeTarget target, Action<IChangeTarget> onClickAction, Action<IChangeTarget> onHoverStart, Action<IChangeTarget> onHoverEnd)
        {
            this._Target = target;
            this._OnClickAction = onClickAction;
            this._OnHoverAction = onHoverStart;
            this._OnHoverEndAction = onHoverEnd;
        }

        public void OnMouseUpAsButton()
        {
            this._OnClickAction?.Invoke(this._Target);
        }

        public void OnMouseEnter()
        {
            this._OnHoverAction?.Invoke(this._Target);
        }

        public void OnMouseExit()
        {
            this._OnHoverEndAction?.Invoke(this._Target);
        }
    }
}