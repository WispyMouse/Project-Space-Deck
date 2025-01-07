namespace SpaceDeck.GameState.Changes
{
    using System;
    using System.Collections;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Utility.Minimum;

    public class StartEntityTurn : GameStateChange
    {
        private readonly Entity MyEntityTarget;

        public StartEntityTurn(Entity turnToStart) : base(turnToStart)
        {
            this.MyEntityTarget = turnToStart;
        }

        public override void Apply(IGameStateMutator toApplyTo)
        {
            toApplyTo.StartEntityTurn(this.MyEntityTarget);
        }

        public override string Describe()
        {
            throw new NotImplementedException();
        }
    }
}