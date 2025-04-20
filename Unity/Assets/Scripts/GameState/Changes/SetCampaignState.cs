namespace SpaceDeck.GameState.Changes
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Utility.Minimum;
    using SpaceDeck.Utility.Wellknown;

    public class SetCampaignState : GameStateChange
    {
        public readonly LowercaseString ToState;

        public SetCampaignState(LowercaseString toState) : base(NobodyTarget.Instance)
        {
            this.ToState = toState;
        }

        public override void Apply(IGameStateMutator toApplyTo)
        {
            toApplyTo.SetCampaignState(this.ToState);
        }

        public override string Describe()
        {
            throw new NotImplementedException();
        }
    }
}