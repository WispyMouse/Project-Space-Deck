namespace SpaceDeck.GameState.Changes
{
    using System;
    using System.Collections;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Models.Databases;
    using SpaceDeck.Tokenization.Evaluatables;
    using SpaceDeck.Utility.Minimum;
    using SpaceDeck.Utility.Wellknown;

    public class ModifyStatusEffectStacks : GameStateChange, IChangeWithIntensity
    {
        public readonly LowercaseString StatusEffect;
        public decimal Intensity { get; set; } = 0;
        public InitialIntensityPositivity Positivity { get; private set; }

        public ModifyStatusEffectStacks(IChangeTarget changeTarget, LowercaseString statusEffect, decimal modifyValue, InitialIntensityPositivity positivity) : base(changeTarget)
        {
            this.StatusEffect = statusEffect;
            this.Intensity = modifyValue;
            this.Positivity = positivity;
        }

        public override void Apply(IGameStateMutator toApplyTo)
        {
            foreach (Entity curEntity in this.Target.GetRepresentedEntities(toApplyTo) ?? Array.Empty<Entity>())
            {
                if (!curEntity.AppliedStatusEffects.TryGetValue(this.StatusEffect, out AppliedStatusEffect existingEffect))
                {
                    existingEffect = StatusEffectDatabase.GetInstance(this.StatusEffect);
                    curEntity.AppliedStatusEffects.Add(this.StatusEffect, existingEffect);
                }

                decimal currentStacks = toApplyTo.GetNumericQuality(existingEffect, WellknownQualities.Stacks, 0);
                decimal newTotal = currentStacks + (int)toApplyTo.GetIntensity(this);
                toApplyTo.SetNumericQuality(existingEffect, WellknownQualities.Stacks, newTotal);
            }
        }

        public override string Describe()
        {
            throw new NotImplementedException();
        }
    }
}