namespace SpaceDeck.GameState.Changes
{
    using System;
    using System.Collections;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Models.Databases;
    using SpaceDeck.Tokenization.Evaluatables;
    using SpaceDeck.Utility.Minimum;
    using SpaceDeck.Utility.Wellknown;

    public class ModifyStatusEffectStacks : GameStateChange
    {
        public readonly LowercaseString StatusEffect;
        public readonly decimal ModifyValue;

        public ModifyStatusEffectStacks(IChangeTarget changeTarget, LowercaseString statusEffect, decimal modifyValue) : base(changeTarget)
        {
            this.ModifyValue = modifyValue;
            this.StatusEffect = statusEffect;
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
                decimal newTotal = currentStacks + this.ModifyValue;
                toApplyTo.SetNumericQuality(existingEffect, WellknownQualities.Stacks, newTotal);
            }
        }

        public override string Describe()
        {
            throw new NotImplementedException();
        }
    }
}