namespace SpaceDeck.GameState.Minimum
{
    using SpaceDeck.Utility.Minimum;
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class Intent : IDescribable
    {
        private static readonly IReadOnlyList<GameStateChange> EmptyList = new List<GameStateChange>();

        public virtual IReadOnlyList<GameStateChange> ActOnIntent(IGameStateMutator mutator)
        {
            return EmptyList;
        }

        public virtual string Describe()
        {
            return GameStateChange.Describe(EmptyList);
        }
    }
}