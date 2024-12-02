namespace SpaceDeck.Tokenization.Evaluatables
{
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.GameState.Context;
    using System.Collections.Generic;
    using SpaceDeck.Tokenization.Minimum.Questions;

    public class DefaultTargetEvaluatableValue : ChangeTargetEvaluatableValue
    {
        public static readonly DefaultTargetEvaluatableValue Instance = new DefaultTargetEvaluatableValue();

        private DefaultTargetEvaluatableValue()
        {

        }

        public override bool TryEvaluate(ExecutionAnswerSet answers, out IChangeTarget value)
        {
            value = null;
            return value != null;
        }
    }
}