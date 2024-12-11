namespace SpaceDeck.Tokenization.Evaluatables
{
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.GameState.Context;
    using SpaceDeck.Tokenization.Minimum.Questions;
    using System.Collections.Generic;
    using SpaceDeck.Tokenization.Evaluatables.Questions;

    public class SpecificTargetEvaluatableValue : ChangeTargetEvaluatableValue
    {
        public readonly IChangeTarget Target;

        public SpecificTargetEvaluatableValue(IChangeTarget target) : base(new SpecificTargetProvider(target))
        {
            this.Target = target;
        }
    }
}