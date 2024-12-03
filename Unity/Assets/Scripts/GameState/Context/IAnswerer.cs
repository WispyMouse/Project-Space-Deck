namespace SpaceDeck.GameState.Context
{
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.Tokenization.Minimum.Questions;
    using System.Collections;
    using System.Collections.Generic;

    public delegate void ProvideQuestionAnswerDelegate(ExecutionAnswerSet answer);
    public delegate void ProvideQuestionsAnswersDelegate(IReadOnlyList<ExecutionAnswerSet> answers);

    public interface IAnswerer
    {
        public void HandleQuestion(ExecutionQuestion question, ProvideQuestionAnswerDelegate answerReceiver);
        public void HandleQuestions(IReadOnlyList<ExecutionQuestion> questions, ProvideQuestionsAnswersDelegate answerReceiver);
    }
}