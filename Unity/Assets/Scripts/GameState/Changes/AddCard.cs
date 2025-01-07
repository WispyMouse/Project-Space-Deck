namespace SpaceDeck.GameState.Changes
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
        using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Tokenization.Evaluatables;
    using SpaceDeck.Utility.Minimum;
    using SpaceDeck.Utility.Wellknown;

    public class AddCard : GameStateChange
    {
        public readonly CardInstance CardInstance;
        public readonly LowercaseString Zone;

        public AddCard(CardInstance cardInstance, LowercaseString zone) : base (NobodyTarget.Instance)
        {
            this.CardInstance = cardInstance;
            this.Zone = zone;
        }

        public override void Apply(IGameStateMutator toApplyTo)
        {
            toApplyTo.AddCard(this.CardInstance, this.Zone);
        }

        public override string Describe()
        {
            throw new NotImplementedException();
        }
    }
}
