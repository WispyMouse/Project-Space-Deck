namespace SpaceDeck.GameState.Changes
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Tokenization.Evaluatables;
    using SpaceDeck.Utility.Minimum;
    using SpaceDeck.Utility.Wellknown;

    public class ShuffleDeck : GameStateChange
    {
        public ShuffleDeck() : base(NobodyTarget.Instance)
        {

        }

        public override void Apply(IGameStateMutator toApplyTo)
        {
            toApplyTo.ShuffleDeck();
        }

        public override string Describe()
        {
            throw new NotImplementedException();
        }
    }
}