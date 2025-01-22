namespace SpaceDeck.Tokenization.Evaluatables.Questions
{
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Tokenization.Evaluatables;
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.Tokenization.Minimum.Context;
    using SpaceDeck.Tokenization.Minimum.Questions;
    using System;
    using System.Collections.Generic;

    public class SpecificCardsProvider : CardInstancesProvider
    {
        public readonly IReadOnlyList<CardInstance> SpecificCards;

        public SpecificCardsProvider(IReadOnlyList<CardInstance> specificCard)
        {
            this.SpecificCards = specificCard;
        }

        public override string Describe()
        {
            return string.Empty;
        }

        public override IReadOnlyList<CardInstance> GetProvidedCards(ScriptingExecutionContext answeringContext)
        {
            return this.SpecificCards;
        }

        public override IReadOnlyList<CardInstance> GetProvidedCards(IGameStateMutator mutator)
        {
            return this.SpecificCards;
        }
    }
}