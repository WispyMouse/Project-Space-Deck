namespace SpaceDeck.GameState.Minimum
{
    using System.Collections;
    using System.Collections.Generic;
    using SpaceDeck.GameState.Minimum;
    using System;

    public class ResolveChange : GameStateChange
    {
        public readonly IResolve Resolve;

        /// <summary>
        /// ResolveChange should only keep its history if the IResolve it is
        /// meant for should keep its history.
        /// </summary>
        public override bool ShouldKeepHistory => this.Resolve.ShouldKeepHistory;

        public ResolveChange(IResolve toResolve) : base(NobodyTarget.Instance)
        {
            this.Resolve = toResolve;

            // ResolveChanges can never be the source of triggers, themselves
            // so it's always appropriate to Apply them in the GameStateDeltaMaker
            this.Triggered = true;
        }

        public override void Apply(IGameStateMutator toApplyTo)
        {
            this.Resolve.Apply(toApplyTo);
        }

        public override string Describe()
        {
            throw new NotImplementedException();
        }
    }
}