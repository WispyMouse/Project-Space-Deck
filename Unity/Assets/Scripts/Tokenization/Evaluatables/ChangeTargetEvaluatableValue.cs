namespace SpaceDeck.Tokenization.Evaluatables
{
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.GameState.Context;
    using System.Collections.Generic;
    using SpaceDeck.Tokenization.Minimum.Questions;

    public abstract class ChangeTargetEvaluatableValue : IEvaluatableValue<IChangeTarget>
    {
        public abstract bool TryEvaluate(ExecutionAnswerSet answers, out IChangeTarget value);
    }
}