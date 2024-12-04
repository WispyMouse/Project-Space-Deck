using System.Collections.Generic;

namespace SpaceDeck.Tokenization.Minimum.Questions
{
    public class ExecutionAnswerSet
    {
        private readonly Dictionary<ExecutionQuestion, ExecutionAnswer> questionsToAnswers = new Dictionary<ExecutionQuestion, ExecutionAnswer>();
        public IEnumerable<ExecutionAnswer> Answers => questionsToAnswers.Values;

        public ExecutionAnswerSet()
        {
        }

        public ExecutionAnswerSet(ExecutionAnswer answer)
        {
            this.questionsToAnswers.Add(answer.Question, answer);
        }

        public ExecutionAnswerSet(IEnumerable<ExecutionAnswer> answers)
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
    }
}