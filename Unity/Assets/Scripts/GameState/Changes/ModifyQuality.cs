namespace SpaceDeck.GameState.Changes
{
    using System.Collections;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Tokenization.Evaluatables;

    public class ModifyQuality : QualityChange
    {
        public readonly decimal ModifyValue;

        public ModifyQuality(IChangeTarget changeTarget, string qualityToChange, decimal modifyValue) : base(changeTarget, qualityToChange)
        {
            this.ModifyValue = modifyValue;
        }

        public override void ApplyToGameState(ref GameState toApplyTo)
        {
            foreach (Entity curEntity in this.Target.GetRepresentedEntities())
            {
                curEntity.SetQuality(this.QualityToChange, curEntity.GetQuality(this.QualityToChange) + ModifyValue);
            }
        }
    }
}