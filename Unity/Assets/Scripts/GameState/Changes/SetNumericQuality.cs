namespace SpaceDeck.GameState.Changes
{
    using System;
    using System.Collections;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Utility.Minimum;

    public class SetNumericQuality : QualityChange
    {
        public readonly decimal NewValue;

        public SetNumericQuality(IChangeTarget changeTarget, IHaveQualities qualitiesHaver, string qualityToChange, decimal newValue) : base(changeTarget, qualitiesHaver, qualityToChange)
        {
            this.NewValue = newValue;
        }

        public override void Apply(IGameStateMutator toApplyTo)
        {
            foreach (Entity curEntity in this.Target.GetRepresentedEntities(toApplyTo))
            {
                curEntity.Qualities.SetNumericQuality(this.QualityToChange, this.NewValue);
            }
        }

        public override string Describe()
        {
            throw new NotImplementedException();
        }
    }
}