namespace SpaceDeck.GameState.Changes
{
    using System;
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

        public override void ApplyToGameState(IGameStateMutator toApplyTo)
        {
            foreach (Entity curEntity in this.Target.GetRepresentedEntities(toApplyTo) ?? Array.Empty<Entity>())
            {
                toApplyTo.SetQuality(curEntity, this.QualityToChange, toApplyTo.GetQuality(curEntity, this.QualityToChange) + ModifyValue);
            }
        }
    }
}