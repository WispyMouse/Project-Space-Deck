namespace SpaceDeck.GameState.Changes
{
    using System;
    using System.Collections;
    using SpaceDeck.GameState.Changes.Targets;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Tokenization.Evaluatables;
    using SpaceDeck.Utility.Minimum;

    public class ModifyElement : GameStateChange
    {
        public readonly LowercaseString ElementId;
        public readonly int ModifyValue;

        public ModifyElement(LowercaseString elementId, int modifyValue) : base(NobodyTarget.Instance)
        {
            this.ElementId = elementId;
            this.ModifyValue = modifyValue;
        }

        public override void Apply(IGameStateMutator toApplyTo)
        {
            toApplyTo.ModifyElement(this.ElementId, this.ModifyValue);
        }

        public override string Describe()
        {
            throw new NotImplementedException();
        }
    }
}