namespace SpaceDeck.Tokenization.Evaluatables.Questions
{
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Tokenization.Minimum.Context;
    using SpaceDeck.Utility.Minimum;

    public abstract class CardInstanceProvider : IDescribable
    {
        public abstract CardInstance GetProvidedCard(ScriptingExecutionContext answeringContext);
        public abstract CardInstance GetProvidedCard(IGameStateMutator mutator);
        public abstract string Describe();
    }
}