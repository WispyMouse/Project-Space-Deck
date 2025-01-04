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
        public abstract CardInstance GetProvidedCard(QuestionAnsweringContext answeringContext);
        public abstract string Describe();
    }
}