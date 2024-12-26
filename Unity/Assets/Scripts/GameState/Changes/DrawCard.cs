namespace SpaceDeck.GameState.Changes
{
    using System;
    using System.Collections;
    using SpaceDeck.GameState.Changes.Targets;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Tokenization.Evaluatables;
    using SpaceDeck.Utility.Minimum;

    public class DrawCard : GameStateChange
    {
        public readonly INumericEvaluatableValue AmountToDraw;

        public DrawCard(INumericEvaluatableValue amountToDraw) : base(NobodyTarget.Instance)
        {
            this.AmountToDraw = amountToDraw;
        }

        public override void Apply(IGameStateMutator toApplyTo)
        {
            throw new NotImplementedException();
        }
    }
}