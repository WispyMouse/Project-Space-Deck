namespace SpaceDeck.GameState.Changes
{
    using System.Collections;
    using SpaceDeck.GameState.Minimum;

    public class ModifyQuality : QualityChange
    {
        public readonly int ModifyValue;

        public ModifyQuality(IChangeTarget changeTarget, string qualityToChange, int modifyValue) : base(changeTarget, qualityToChange)
        {
            this.ModifyValue = modifyValue;
        }
    }
}