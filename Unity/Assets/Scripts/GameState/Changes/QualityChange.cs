namespace SpaceDeck.GameState.Changes
{
    using System;
    using System.Collections;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Utility.Minimum;

    public abstract class QualityChange : GameStateChange
    {
        public readonly LowercaseString QualityToChange;
        public readonly IHaveQualities QualitiesHaver;

        public QualityChange(IChangeTarget changeTarget, IHaveQualities qualityHaver, LowercaseString qualityToChange) : base(changeTarget)
        {
            this.QualityToChange = qualityToChange;
            this.QualitiesHaver = qualityHaver;
        }

        public override string Describe()
        {
            throw new NotImplementedException();
        }
    }
}