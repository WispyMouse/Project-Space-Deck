namespace SpaceDeck.GameState.Context
{
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.Tokenization.Minimum.Context;
    using SpaceDeck.Tokenization.Minimum.Questions;
    using System.Collections;
    using System.Collections.Generic;

    public delegate void ProvideQuestionAnswerDelegate(ExecutionAnswer answer);
    public delegate void ProvideQuestionsAnswersDelegate(ExecutionAnswerSet answers);

    public interface IAnswerer
    {
        public void HandleQuestion(ExecutionQuestion question, ProvideQuestionAnswerDelegate answerReceiver);
        public void HandleQuestions(IReadOnlyList<ExecutionQuestion> questions, ProvideQuestionsAnswersDelegate answerReceiver);
    }
}