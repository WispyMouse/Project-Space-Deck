namespace SpaceDeck.Tokenization.Evaluatables.Questions
{
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Tokenization.Evaluatables;
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.Tokenization.Minimum.Context;
    using SpaceDeck.Tokenization.Minimum.Questions;
    using SpaceDeck.Utility.Minimum;
    using System.Collections.Generic;

    public abstract class CardInstancesProvider : IDescribable
    {
        public abstract IReadOnlyList<CardInstance> GetProvidedCards(ScriptingExecutionContext answeringContext);
        public abstract IReadOnlyList<CardInstance> GetProvidedCards(IGameStateMutator mutator);
        public abstract string Describe();
    }
}