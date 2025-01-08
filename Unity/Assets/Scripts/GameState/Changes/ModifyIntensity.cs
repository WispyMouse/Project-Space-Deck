namespace SpaceDeck.GameState.Changes
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using SpaceDeck.GameState.Minimum;

    /// <summary>
    /// Reduces the intensity of an associated triggered action.
    /// Everything that can have a value has it reduced, to a minimum of zero.
    /// If an intensity is already negative, it won't be changed.
    /// </summary>
    public class ModifyIntensity : GameStateChange, IChangeWithIntensity
    {
        public readonly GameStateChange AppliedToChange;
        public InitialIntensityPositivity Positivity { get; private set; }

        public decimal Intensity { get; set; } = 0;

        public ModifyIntensity(GameStateChange change, decimal intensity, InitialIntensityPositivity positivity) : base(change.Target)
        {
            this.AppliedToChange = change;
            this.Intensity = intensity;
            this.Positivity = positivity;
        }

        public override void Apply(IGameStateMutator toApplyTo)
        {
            if (this.AppliedToChange is IChangeWithIntensity changeWithIntensity)
            {
                if (changeWithIntensity.Positivity == InitialIntensityPositivity.PositiveOrZero)
                {
                    changeWithIntensity.Intensity = Math.Max(0, changeWithIntensity.Intensity + this.Intensity);
                }
                else
                {
                    changeWithIntensity.Intensity = Math.Min(0, changeWithIntensity.Intensity + this.Intensity);
                }
            }
        }

        public override string Describe()
        {
            throw new NotImplementedException();
        }
    }
}