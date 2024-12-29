namespace SpaceDeck.UX
{
    using SFDDCards;
    using SFDDCards.ScriptingTokens;
    using SpaceDeck.GameState.Minimum;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class TargetableIndicator : MonoBehaviour
    {
        [Obsolete("Transition to " + nameof(_Target))]
        ICombatantTarget Target { get; set; }
        [Obsolete("Transition to " + nameof(_OnClickAction))]
        Action<ICombatantTarget> OnClickAction { get; set; } = null;
        [Obsolete("Transition to " + nameof(_OnHoverAction))]
        Action<ICombatantTarget> OnHoverAction { get; set; } = null;
        [Obsolete("Transition to " + nameof(_OnHoverEndAction))]
        Action<ICombatantTarget> OnHoverEndAction { get; set; } = null;

        IChangeTarget _Target { get; set; }
        Action<IChangeTarget> _OnClickAction { get; set; } = null;
        Action<IChangeTarget> _OnHoverAction { get; set; } = null;
        Action<IChangeTarget> _OnHoverEndAction { get; set; } = null;

        [Obsolete("Transition to " + nameof(_SetFromTarget))]
        public void SetFromTarget(ICombatantTarget target, Action<ICombatantTarget> onClickAction, Action<ICombatantTarget> onHoverStart, Action<ICombatantTarget> onHoverEnd)
        {
            this.Target = target;
            this.OnClickAction = onClickAction;
            this.OnHoverAction = onHoverStart;
            this.OnHoverEndAction = onHoverEnd;
        }

        public void _SetFromTarget(IChangeTarget target, Action<IChangeTarget> onClickAction, Action<IChangeTarget> onHoverStart, Action<IChangeTarget> onHoverEnd)
        {
            this._Target = target;
            this._OnClickAction = onClickAction;
            this._OnHoverAction = onHoverStart;
            this._OnHoverEndAction = onHoverEnd;
        }

        public void OnMouseUpAsButton()
        {
            this.OnClickAction?.Invoke(this.Target);
            this._OnClickAction?.Invoke(this._Target);
        }

        public void OnMouseEnter()
        {
            this.OnHoverAction?.Invoke(this.Target);
            this._OnHoverAction?.Invoke(this._Target);
        }

        public void OnMouseExit()
        {
            this.OnHoverEndAction?.Invoke(this.Target);
            this._OnHoverEndAction?.Invoke(this._Target);
        }
    }
}