namespace SpaceDeck.GameState.Changes
{
    using System.Collections;
    using SpaceDeck.GameState.Changes.Targets;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Utility.Minimum;

    public class EndCurrentFactionTurn : GameStateChange
    {
        public EndCurrentFactionTurn() : base(NobodyTarget.Instance)
        {
        }

        public override void Apply(IGameStateMutator toApplyTo)
        {
            toApplyTo.EndCurrentFactionTurn();
        }
    }
}