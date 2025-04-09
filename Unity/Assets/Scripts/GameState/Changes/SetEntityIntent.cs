namespace SpaceDeck.GameState.Changes
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using SpaceDeck.GameState.Minimum;

    public class SetEntityIntent : GameStateChange
    {
        public readonly Intent SetToIntent;

        public SetEntityIntent(IChangeTarget target, Intent to) : base(target)
        {
            this.SetToIntent = to;
        }

        public override void Apply(IGameStateMutator toApplyTo)
        {
            foreach (Entity curEntity in new List<Entity>(this.Target.GetRepresentedEntities(toApplyTo)))
            {
                curEntity.CurrentIntent = this.SetToIntent;
            }
        }

        public override string Describe()
        {
            throw new NotImplementedException();
        }
    }
}