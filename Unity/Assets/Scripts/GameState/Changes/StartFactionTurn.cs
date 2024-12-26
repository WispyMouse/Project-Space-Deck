namespace SpaceDeck.GameState.Changes
{
    using System.Collections;
    using SpaceDeck.GameState.Changes.Targets;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Utility.Minimum;

    public class StartFactionTurn : GameStateChange
    {
        public readonly decimal FactionToChangeTo;

        public StartFactionTurn(decimal factionToChangeTo) : base(NobodyTarget.Instance)
        {
            this.FactionToChangeTo = factionToChangeTo;
        }

        public override void Apply(IGameStateMutator toApplyTo)
        {
            toApplyTo.StartFactionTurn(this.FactionToChangeTo);
        }
    }
}