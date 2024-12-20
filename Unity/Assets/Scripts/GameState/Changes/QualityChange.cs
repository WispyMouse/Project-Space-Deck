namespace SpaceDeck.GameState.Changes
{
    using System.Collections;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Utility.Minimum;

    public abstract class QualityChange : GameStateChange
    {
        public readonly LowercaseString QualityToChange;

        public QualityChange(IChangeTarget changeTarget, LowercaseString qualityToChange) : base(changeTarget)
        {
            this.QualityToChange = qualityToChange;
        }
    }
}