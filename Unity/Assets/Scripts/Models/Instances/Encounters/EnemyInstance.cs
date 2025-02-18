namespace SpaceDeck.Models.Instances
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Models.Prototypes;
    using SpaceDeck.Utility.Minimum;
    using SpaceDeck.Utility.Wellknown;

    public class EnemyInstance : Entity
    {
        public readonly LowercaseString Id;

        public Dictionary<LowercaseString, EnemyAttack> Attacks = new Dictionary<LowercaseString, EnemyAttack>();

        public EnemyInstance(EnemyPrototype prototype)
        {
            this.Id = prototype.Id;
            this.Qualities.AddQualities(prototype.Qualities);

            // Set this instance's "Health" to the provided qualities "Health" value
            // If no value for "Health" is provided (this is usual), then fallback to MaximumHealth.
            // If there is no MaximumHealth, then this is probably a misformed object, but we can fallback to 0.
            this.Qualities.SetNumericQuality(WellknownQualities.Health, 
                this.Qualities.GetNumericQuality(WellknownQualities.Health, 
                    this.Qualities.GetNumericQuality(WellknownQualities.MaximumHealth)));
        }

        public override EnemyAttack GetNextAttack(RandomDecider<EnemyAttack> decider = null)
        {
            if (decider == null)
            {
                decider = new RandomDecider<EnemyAttack>();
            }

            if (this.Attacks.Count == 0)
            {
                return null;
            }

            return decider.ChooseRandomly(new List<EnemyAttack>(this.Attacks.Values));
        }
    }
}