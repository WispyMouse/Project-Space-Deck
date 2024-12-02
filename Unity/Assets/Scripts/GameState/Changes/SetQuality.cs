namespace SpaceDeck.GameState.Changes
{
    using System.Collections;
    using SpaceDeck.GameState.Minimum;

    public class SetQuality : QualityChange
    {
        public readonly decimal NewValue;

        public SetQuality(IChangeTarget changeTarget, string qualityToChange, decimal newValue) : base(changeTarget, qualityToChange)
        {
            this.NewValue = newValue;
        }

        public override void ApplyToGameState(ref GameState toApplyTo)
        {
            foreach (Entity curEntity in this.Target.GetRepresentedEntities())
            {
                curEntity.SetQuality(this.QualityToChange, this.NewValue);
            }
        }
    }
}