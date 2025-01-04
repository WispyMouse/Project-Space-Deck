namespace SpaceDeck.Tokenization.Evaluatables.Questions
{
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Tokenization.Evaluatables;
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.Tokenization.Minimum.Context;
    using SpaceDeck.Tokenization.Minimum.Questions;
    using System;
    using System.Collections.Generic;

    public class PlayedCardProvider : CardInstanceProvider
    {
        public static readonly PlayedCardProvider Instance = new PlayedCardProvider();

        private PlayedCardProvider()
        {

        }

        public override CardInstance GetProvidedCard(QuestionAnsweringContext answeringContext)
        {
            return answeringContext?.PlayingCard;
        }

        public override string Describe()
        {
            return string.Empty;
        }
    }
}