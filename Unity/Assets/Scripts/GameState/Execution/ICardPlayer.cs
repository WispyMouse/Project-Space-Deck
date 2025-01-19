namespace SpaceDeck.GameState.Execution
{
    using SpaceDeck.GameState.Changes;
    using SpaceDeck.GameState.Context;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.Tokenization.Minimum.Context;
    using SpaceDeck.Tokenization.Minimum.Questions;
    using SpaceDeck.Utility.Wellknown;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public interface ICardPlayer
    {
        QuestionAnsweringContext StartConsideringPlayingCard(CardInstance toPlay);
        bool TryGetCurrentQuestions(out IReadOnlyList<ExecutionQuestion> questions);
        bool TryExecuteCurrentCard(ExecutionAnswerSet answers);
    }
}