namespace SpaceDeck.GameState.Changes
{
    using System;
    using System.Collections;
    using SpaceDeck.GameState.Changes.Targets;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Utility.Minimum;

    public class ActionExecutor : GameStateChange
    {
        public readonly Action<IGameStateMutator> ToExecute;

        public ActionExecutor(Action<IGameStateMutator> toExecute) : base(NobodyTarget.Instance)
        {
            this.ToExecute = toExecute;
        }

        public override void Apply(IGameStateMutator toApplyTo)
        {
            this.ToExecute(toApplyTo);
        }

        public override string Describe()
        {
            throw new NotImplementedException();
        }
    }
}