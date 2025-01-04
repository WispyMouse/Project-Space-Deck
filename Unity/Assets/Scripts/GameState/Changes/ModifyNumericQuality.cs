namespace SpaceDeck.GameState.Changes
{
    using System;
    using System.Collections;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Tokenization.Evaluatables;
    using SpaceDeck.Utility.Minimum;

    public class ModifyNumericQuality : QualityChange
    {
        public readonly decimal ModifyValue;

        public ModifyNumericQuality(IChangeTarget changeTarget, IHaveQualities qualitiesHaver, LowercaseString qualityToChange, decimal modifyValue) : base(changeTarget, qualitiesHaver, qualityToChange)
        {
            this.ModifyValue = modifyValue;
        }

        public override void Apply(IGameStateMutator toApplyTo)
        {
            foreach (Entity curEntity in this.Target.GetRepresentedEntities(toApplyTo) ?? Array.Empty<Entity>())
            {
                toApplyTo.SetNumericQuality(curEntity, this.QualityToChange, toApplyTo.GetNumericQuality(curEntity, this.QualityToChange) + ModifyValue);
            }
        }
    }
}