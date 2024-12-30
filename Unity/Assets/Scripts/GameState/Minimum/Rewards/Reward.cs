namespace SpaceDeck.GameState.Minimum
{
    using SpaceDeck.Utility.Minimum;
    using System;
    using System.Collections.Generic;

    public class Reward
    {
        public enum PickRewardProtocol
        {
            ChooseX
        }

        public readonly LowercaseString Id;

        public Reward(LowercaseString id)
        {
            this.Id = id;
        }
    }
}