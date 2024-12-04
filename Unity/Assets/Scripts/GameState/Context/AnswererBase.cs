namespace SpaceDeck.GameState.Context
{
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.Tokenization.Minimum.Questions;
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    public abstract class AnswererBase : IAnswerer
    {
        public abstract void HandleQuestion(ExecutionQuestion question, ProvideQuestionAnswerDelegate answerReceiver);

        public virtual void HandleQuestions(IReadOnlyList<ExecutionQuestion> questions, ProvideQuestionsAnswersDelegate answerReceiver)
        {
            if (questions.Count == 0)
            {
                answerReceiver.Invoke(new ExecutionAnswerSet());
                return;
            }

            ConcurrentBag<ExecutionAnswer> answers = new ConcurrentBag<ExecutionAnswer>();

            ProvideQuestionAnswerDelegate provideAnswer = (ExecutionAnswer answer) =>
            {
                lock (answers)
                {
                    answers.Add(answer);
                    if (answers.Count == questions.Count)
                    {
                        answerReceiver.Invoke(new ExecutionAnswerSet(answers));
                        return;
                    }
                }
            };

            foreach (ExecutionQuestion curQuestion in questions)
            {
                this.HandleQuestion(curQuestion, provideAnswer);
            }
        }
    }
}