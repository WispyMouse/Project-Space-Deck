namespace SpaceDeck.Models.Instances
{
    using System.Collections;
    using System.Collections.Generic;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Models.Prototypes;
    using SpaceDeck.Utility.Minimum;

    public class RewardInstance : Reward
    {
        public RewardInstance(RewardPrototype prototype) : base(prototype.Id, prototype.IdentityKind)
        {
        }
    }
}