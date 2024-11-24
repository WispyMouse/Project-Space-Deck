namespace SpaceDeck.GameState.Changes
{
    using System.Collections;
    using SpaceDeck.GameState.Minimum;

    public abstract class QualityChange : GameStateChange
    {
        public readonly string QualityToChange;

        public QualityChange(IChangeTarget changeTarget, string qualityToChange) : base(changeTarget)
        {
            this.QualityToChange = qualityToChange;
        }
    }
}