using SpaceDeck.GameState.Minimum;
using SpaceDeck.Tokenization.Minimum.Context;
using SpaceDeck.Utility.Logging;
using SpaceDeck.Utility.Wellknown;
using System.Collections.Generic;

namespace SpaceDeck.Tokenization.Minimum.Questions
{
    public class ExecutionAnswerSet
    {
        private readonly Dictionary<ExecutionQuestion, ExecutionAnswer> questionsToAnswers = new Dictionary<ExecutionQuestion, ExecutionAnswer>();
        public IEnumerable<ExecutionAnswer> Answers => questionsToAnswers.Values;
        public readonly Entity User;

        public ExecutionAnswerSet(Entity user)
        {
            this.User = user;
        }

        public ExecutionAnswerSet(ExecutionAnswer answer, Entity user) : this(user)
        {
            this.questionsToAnswers.Add(answer.Question, answer);
        }

        public ExecutionAnswerSet(IEnumerable<ExecutionAnswer> answers, Entity user) : this(user)
        {
            foreach (ExecutionAnswer answer in answers)
            {
                if (this.questionsToAnswers.ContainsKey(answer.Question))
                {
                    // TODO: LOG ERROR
                    continue;
                }

                this.questionsToAnswers.Add(answer.Question, answer);
            }
        }

        public bool TryGetAnswerForQuestion(ExecutionQuestion question, out ExecutionAnswer associatedAnswer)
        {
            return this.questionsToAnswers.TryGetValue(question, out associatedAnswer);
        }

        public bool TryGetTypedAnswerForQuestion<Q,A>(Q question, out A associatedAnswer) where Q : ExecutionQuestion where A : ExecutionAnswer<Q>
        {
            if (!this.TryGetAnswerForQuestion(question, out ExecutionAnswer unboxedAssociatedAnswer))
            {
                associatedAnswer = null;
                return false;
            }

            associatedAnswer = unboxedAssociatedAnswer as A;
            return associatedAnswer != null;
        }

        public bool TryGetTypedQuestion<T>(out T foundQuestion) where T : ExecutionQuestion
        {
            foreach (ExecutionQuestion question in this.questionsToAnswers.Keys)
            {
                if (question is T foundQuestionBoxed)
                {
                    foundQuestion = foundQuestionBoxed;
                    return true;
                }
            }

            foundQuestion = null;
            return false;
        }

        public bool TryGetTypedAnswerForQuestionType<Q,A>(out Q foundQuestion, out A foundAnswer) where Q : ExecutionQuestion where A : ExecutionAnswer<Q>
        {
            if (!this.TryGetTypedQuestion<Q>(out foundQuestion) || !this.TryGetTypedAnswerForQuestion<Q,A>(foundQuestion, out foundAnswer))
            {
                foundAnswer = null;
                return false;
            }

            return true;
        }

        public void AddAnswer(ExecutionQuestion question, ExecutionAnswer answer)
        {
            this.questionsToAnswers.Add(question, answer);
        }

        public void SetDefaultAnswers(IReadOnlyList<ExecutionQuestion> questions, QuestionAnsweringContext questionAnsweringContext)
        {
            foreach (ExecutionQuestion question in questions)
            {
                // As we are executing the question answering context, set the default user inside it
                // such that questions that want the "default" answer update to that
                // Do this even if the question is answered already
                // This is the primary reason that the questions need to be ordered, so that earlier questions
                // can supply default answers to later ones
                question.ApplyDefaultToContext(questionAnsweringContext);

                if (this.TryGetAnswerForQuestion(question, out _))
                {
                    // Already have an answer
                    continue;
                }

                if (!question.TryGetDefaultAnswer(questionAnsweringContext, out ExecutionAnswer answer))
                {
                    Logging.DebugLog(WellknownLoggingLevels.Error,
                        WellknownLoggingCategories.EvaluatableEvaluation,
                        $"Cannot get default answer for action.");
                    return;
                }

                this.AddAnswer(question, answer);
            }
        }
    }
}