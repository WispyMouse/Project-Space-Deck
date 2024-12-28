namespace SpaceDeck.GameState.Changes
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using SpaceDeck.GameState.Changes.Targets;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Tokenization.Evaluatables;
    using SpaceDeck.Utility.Minimum;
    using SpaceDeck.Utility.Wellknown;

    public class MoveCard : GameStateChange
    {
        public readonly CardInstance CardToMove;
        public LowercaseString Zone;

        public MoveCard(CardInstance toMove, LowercaseString targetZone) : base (NobodyTarget.Instance)
        {
            this.CardToMove = toMove;
            this.Zone = targetZone;
        }

        public override void Apply(IGameStateMutator toApplyTo)
        {
            toApplyTo.MoveCard(this.CardToMove, this.Zone);
        }

        public override string Describe()
        {
            throw new NotImplementedException();
        }
    }
}