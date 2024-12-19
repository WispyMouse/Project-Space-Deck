namespace SpaceDeck.GameState.Context
{
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.Tokenization.Minimum.Context;
    using SpaceDeck.Tokenization.Minimum.Questions;
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    public abstract class AnswererBase : IAnswerer
    {
        public abstract void HandleQuestion(QuestionAnsweringContext answeringContext, ExecutionQuestion question, ProvideQuestionAnswerDelegate answerReceiver);

        public void HandleQuestions(QuestionAnsweringContext answeringContext, IReadOnlyList<ExecutionQuestion> questions, ProvideQuestionsAnswersDelegate answerReceiver)
        {
            if (questions.Count == 0)
            {
                answerReceiver.Invoke(new ExecutionAnswerSet());
                return;
            }

            // Ask the first question
            this.HandleNextQuestion(answeringContext, questions, 0, answerReceiver, new List<ExecutionAnswer>());
        }

        void HandleNextQuestion(QuestionAnsweringContext answeringContext, IReadOnlyList<ExecutionQuestion> questions, int questionIndex, ProvideQuestionsAnswersDelegate answerReceiver, List<ExecutionAnswer> answers)
        {
            ProvideQuestionAnswerDelegate provideAnswer = (ExecutionAnswer answer) =>
            {
                answers.Add(answer);

                if (answers.Count == questions.Count)
                {
                    answerReceiver.Invoke(new ExecutionAnswerSet(answers));
                    return;
                }

                answer.ApplyToQuestionAnsweringContext(answeringContext);

                this.HandleNextQuestion(answeringContext, questions, questionIndex+1, answerReceiver, answers);
            };

            ExecutionQuestion curQuestion = questions[questionIndex];

            if (curQuestion.TryGetDefaultAnswer(answeringContext, out ExecutionAnswer answer))
            {
                provideAnswer.Invoke(answer);
            }
            else
            {
                this.HandleQuestion(answeringContext, questions[questionIndex], provideAnswer);
            }
        }
    }
}