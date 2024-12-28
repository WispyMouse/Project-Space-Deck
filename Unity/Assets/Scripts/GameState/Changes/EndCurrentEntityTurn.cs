namespace SpaceDeck.GameState.Changes
{
    using System;
    using System.Collections;
    using SpaceDeck.GameState.Changes.Targets;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Utility.Minimum;

    public class EndCurrentEntityTurn : GameStateChange
    {
        public EndCurrentEntityTurn() : base(NobodyTarget.Instance)
        {
        }

        public override void Apply(IGameStateMutator toApplyTo)
        {
            toApplyTo.EndCurrentEntityTurn();
        }

        public override string Describe()
        {
            throw new NotImplementedException();
        }
    }
}