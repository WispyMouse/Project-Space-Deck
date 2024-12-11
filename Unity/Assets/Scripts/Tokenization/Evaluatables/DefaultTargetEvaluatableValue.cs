namespace SpaceDeck.Tokenization.Evaluatables
{
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.GameState.Context;
    using System.Collections.Generic;
    using SpaceDeck.Tokenization.Minimum.Questions;
    using SpaceDeck.Tokenization.Evaluatables.Questions;

    public class DefaultTargetEvaluatableValue : ChangeTargetEvaluatableValue
    {
        public static readonly DefaultTargetEvaluatableValue Instance = new DefaultTargetEvaluatableValue();

        private DefaultTargetEvaluatableValue() : base(DefaultTargetProvider.Instance)
        {

        }
    }
}