namespace SpaceDeck.GameState.Changes
{
    using System.Collections;
    using System.Collections.Generic;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.GameState.Changes.Targets;
    using System;

    public class ResolveChange : GameStateChange
    {
        public readonly IResolve Resolve;

        public ResolveChange(IResolve toResolve) : base(NobodyTarget.Instance)
        {
            this.Resolve = toResolve;
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