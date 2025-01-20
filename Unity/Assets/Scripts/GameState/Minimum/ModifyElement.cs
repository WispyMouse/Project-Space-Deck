namespace SpaceDeck.GameState.Minimum
{
    using System;
    using System.Collections;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Utility.Minimum;

    public class ModifyElement : GameStateChange, IChangeWithIntensity
    {
        public decimal Intensity { get; set; } = 0;
        public InitialIntensityPositivity Positivity { get; private set; }

        public readonly Element ElementToModify;

        public ModifyElement(Element element, int modifyValue, InitialIntensityPositivity positivity) : base(NobodyTarget.Instance)
        {
            this.ElementToModify = element;
            this.Intensity = modifyValue;
            this.Positivity = positivity;
        }

        public override void Apply(IGameStateMutator toApplyTo)
        {
            toApplyTo.ModifyElement(this.ElementToModify, (int)toApplyTo.GetIntensity(this));
        }

        public override string Describe()
        {
            throw new NotImplementedException();
        }
    }
}