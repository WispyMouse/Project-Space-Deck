namespace SpaceDeck.GameState.Changes
{
    using System;
    using System.Collections;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Tokenization.Evaluatables;
    using SpaceDeck.Utility.Minimum;

    public class ModifyCurrency : GameStateChange, IChangeWithIntensity
    {
        public decimal Intensity { get; set; } = 0;
        public InitialIntensityPositivity Positivity { get; private set; }

        public readonly Currency CurrencyToModify;

        public ModifyCurrency(Currency modifyCurrency, int modifyValue, InitialIntensityPositivity positivity) : base(NobodyTarget.Instance)
        {
            this.CurrencyToModify = modifyCurrency;
            this.Intensity = modifyValue;
            this.Positivity = positivity;
        }

        public override void Apply(IGameStateMutator toApplyTo)
        {
            toApplyTo.ModCurrency(this.CurrencyToModify, (int)this.Intensity);
        }

        public override string Describe()
        {
            throw new NotImplementedException();
        }
    }
}