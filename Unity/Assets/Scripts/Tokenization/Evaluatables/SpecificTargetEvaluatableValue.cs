namespace SpaceDeck.Tokenization.Evaluatables
{
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.GameState.Context;
    using SpaceDeck.Tokenization.Minimum.Questions;
    using System.Collections.Generic;

    public class SpecificTargetEvaluatableValue : ChangeTargetEvaluatableValue
    {
        public readonly IChangeTarget Target;

        public SpecificTargetEvaluatableValue(IChangeTarget target)
        {
            this.Target = target;
        }
    }
}