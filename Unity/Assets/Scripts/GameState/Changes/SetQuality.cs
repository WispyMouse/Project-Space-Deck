namespace SpaceDeck.GameState.Changes
{
    using System.Collections;
    using SpaceDeck.GameState.Minimum;

    public class SetQuality : QualityChange
    {
        public readonly int NewValue;

        public SetQuality(IChangeTarget changeTarget, string qualityToChange, int newValue) : base(changeTarget, qualityToChange)
        {
            this.NewValue = newValue;
        }
    }
}