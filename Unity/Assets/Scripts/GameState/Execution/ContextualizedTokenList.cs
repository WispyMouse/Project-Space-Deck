namespace SpaceDeck.GameState.Execution
{
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.Tokenization.Minimum.Questions;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a token list with the appropriate play context
    /// to start representing the necessary questions to answer
    /// in order to play the card.
    /// 
    /// When this is provided with answers, it can be parsed
    /// into a <see cref="GameState.Minimum.GameStateDelta"/>.
    /// </summary>
    public struct ContextualizedTokenList
    {
        public readonly LinkedTokenList Tokens;
        public readonly IEnumerable<LinkedExecutionQuestion> Questions;

        public ContextualizedTokenList(LinkedTokenList tokens)
        {
            this.Tokens = tokens;

            List<LinkedExecutionQuestion> questions = new List<LinkedExecutionQuestion>();

            LinkedToken nextToken = this.Tokens.First;
            while (nextToken != null)
            {
                foreach (ExecutionQuestion question in nextToken.Questions)
                {
                    questions.Add(new LinkedExecutionQuestion(nextToken, question));
                }
            }

            this.Questions = questions;
        }
    }
}