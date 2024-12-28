namespace SpaceDeck.GameState.Changes
{
    using System;
    using System.Collections;
    using SpaceDeck.GameState.Minimum;

    public class SetQuality : QualityChange
    {
        public readonly decimal NewValue;

        public SetQuality(IChangeTarget changeTarget, string qualityToChange, decimal newValue) : base(changeTarget, qualityToChange)
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