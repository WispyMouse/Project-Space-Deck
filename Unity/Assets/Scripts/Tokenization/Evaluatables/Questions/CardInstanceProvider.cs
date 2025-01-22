namespace SpaceDeck.Tokenization.Evaluatables.Questions
{
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Tokenization.Evaluatables;
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.Tokenization.Minimum.Context;
    using SpaceDeck.Tokenization.Minimum.Questions;
    using SpaceDeck.Utility.Minimum;
    using System.Collections.Generic;

    public abstract class CardInstanceProvider : IDescribable
    {
        public abstract CardInstance GetProvidedCard(ScriptingExecutionContext answeringContext);
        public abstract CardInstance GetProvidedCard(IGameStateMutator mutator);
        public abstract string Describe();
    }
}