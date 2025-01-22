namespace SpaceDeck.Tokenization.Evaluatables.Questions
{
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Tokenization.Evaluatables;
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.Tokenization.Minimum.Context;
    using SpaceDeck.Tokenization.Minimum.Questions;
    using System;
    using System.Collections.Generic;

    public class SpecificCardProvider : CardInstanceProvider
    {
        public readonly CardInstance SpecificCard;

        public SpecificCardProvider(CardInstance specificCard)
        {
            this.SpecificCard = specificCard;
        }

        public override string Describe()
        {
            return string.Empty;
        }

        public override CardInstance GetProvidedCard(ScriptingExecutionContext answeringContext)
        {
            return this.SpecificCard;
        }

        public override CardInstance GetProvidedCard(IGameStateMutator mutator)
        {
            return this.SpecificCard;
        }
    }
}