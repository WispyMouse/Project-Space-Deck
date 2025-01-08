namespace SpaceDeck.GameState.Changes
{
    using System;
    using System.Collections;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Tokenization.Evaluatables;
    using SpaceDeck.Utility.Minimum;
    using SpaceDeck.Utility.Wellknown;

    public class ModifyNumericQuality : QualityChange, IChangeWithIntensity
    {
        public decimal Intensity { get; set; } = 0;
        public InitialIntensityPositivity Positivity { get; private set; }

        public ModifyNumericQuality(IChangeTarget changeTarget, IHaveQualities qualitiesHaver, LowercaseString qualityToChange, decimal modifyValue, InitialIntensityPositivity positivity) : base(changeTarget, qualitiesHaver, qualityToChange)
        {
            this.Intensity = modifyValue;
            this.Positivity = positivity;
        }

        public override void Apply(IGameStateMutator toApplyTo)
        {
            foreach (Entity curEntity in this.Target.GetRepresentedEntities(toApplyTo) ?? Array.Empty<Entity>())
            {
                toApplyTo.SetNumericQuality(curEntity, this.QualityToChange, toApplyTo.GetNumericQuality(curEntity, this.QualityToChange) + this.Intensity);
            }
        }

        public override void Trigger(IGameStateMutator toPushTriggers)
        {
            GameStateEventTrigger trigger = new GameStateEventTrigger(WellknownGameStateEvents.GetQualityAffected(this.QualityToChange), this);
            toPushTriggers.TriggerAndStack(trigger);
        }
    }
}