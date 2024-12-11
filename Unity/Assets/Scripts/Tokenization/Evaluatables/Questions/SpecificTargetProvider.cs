namespace SpaceDeck.Tokenization.Evaluatables.Questions
{
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Tokenization.Evaluatables;
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.Tokenization.Minimum.Questions;
    using System.Collections.Generic;

    public class SpecificTargetProvider : ChangeTargetProvider
    {
        private IReadOnlyList<IChangeTarget> Options;

        public SpecificTargetProvider(IChangeTarget target)
        {
            this.Options = new List<IChangeTarget>() { target };
        }

        public SpecificTargetProvider(IReadOnlyList<IChangeTarget> target)
        {
            this.Options = target;
        }

        public override IChangeTarget ChooseByIndex(int index)
        {
            return this.Options[index];
        }
    }
}