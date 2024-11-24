namespace SpaceDeck.GameState.Changes
{
    using System.Collections;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Tokenization.Minimum.Evaluatables;

    public class ModifyQuality : QualityChange
    {
        public readonly INumericEvaluatableValue ModifyValue;

        public ModifyQuality(IChangeTarget changeTarget, string qualityToChange, INumericEvaluatableValue modifyValue) : base(changeTarget, qualityToChange)
        {
            this.ModifyValue = modifyValue;
        }
    }
}