namespace SpaceDeck.GameState.Changes
{
    using System;
    using System.Collections;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Tokenization.Evaluatables;
    using SpaceDeck.Utility.Minimum;

    public class ModifyElement : GameStateChange, IChangeWithIntensity
    {
        public decimal Intensity { get; set; } = 0;
        public InitialIntensityPositivity Positivity { get; private set; }

        public readonly LowercaseString ElementId;

        public ModifyElement(LowercaseString elementId, int modifyValue, InitialIntensityPositivity positivity) : base(NobodyTarget.Instance)
        {
            this.ElementId = elementId;
            this.Intensity = modifyValue;
            this.Positivity = positivity;
        }

        public override void Apply(IGameStateMutator toApplyTo)
        {
            toApplyTo.ModifyElement(this.ElementId, (int)this.Intensity);
        }

        public override string Describe()
        {
            throw new NotImplementedException();
        }
    }
}