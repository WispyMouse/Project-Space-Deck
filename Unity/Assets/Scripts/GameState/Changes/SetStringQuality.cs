namespace SpaceDeck.GameState.Changes
{
    using System;
    using System.Collections;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Utility.Minimum;

    public class SetStringQuality : QualityChange
    {
        public readonly string NewValue;

        public SetStringQuality(IChangeTarget changeTarget, IHaveQualities qualitiesHaver, string qualityToChange, string newValue) : base(changeTarget, qualitiesHaver, qualityToChange)
        {
            this.NewValue = newValue;
        }

        public override void Apply(IGameStateMutator toApplyTo)
        {
            foreach (Entity curEntity in this.Target.GetRepresentedEntities(toApplyTo))
            {
                curEntity.Qualities.SetStringQuality(this.QualityToChange, this.NewValue);
            }
        }

        public override string Describe()
        {
            throw new NotImplementedException();
        }
    }
}