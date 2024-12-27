namespace SpaceDeck.Tokenization.Evaluatables.Questions
{
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Tokenization.Evaluatables;
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.Tokenization.Minimum.Context;
    using SpaceDeck.Tokenization.Minimum.Questions;
    using System.Collections.Generic;

    public abstract class ChangeTargetProvider
    {
        public abstract IReadOnlyList<IChangeTarget> GetProvidedTargets(QuestionAnsweringContext answeringContext);
        public abstract IReadOnlyList<IChangeTarget> GetProvidedTargets(IGameStateMutator mutator);
    }
}