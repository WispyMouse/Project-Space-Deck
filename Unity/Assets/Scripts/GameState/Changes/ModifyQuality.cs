namespace SpaceDeck.GameState.Changes
{
    using System.Collections;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Tokenization.Evaluatables;

    public class ModifyQuality : QualityChange
    {
        public readonly INumericEvaluatableValue ModifyValue;

        public ModifyQuality(IChangeTarget changeTarget, string qualityToChange, INumericEvaluatableValue modifyValue) : base(changeTarget, qualityToChange)
        {
            this.ModifyValue = modifyValue;
        }

        public override void ApplyToGameState(ref GameState toApplyTo, ref ExecutionContext executionContext)
        {
            if (!this.ModifyValue.TryEvaluate(executionContext, out decimal modifyValueEvaluated))
            {
                return;
            }

            foreach (Entity curEntity in this.Target.GetRepresentedEntities())
            {
                curEntity.SetQuality(this.QualityToChange, curEntity.GetQuality(this.QualityToChange) + modifyValueEvaluated);
            }
        }
    }
}