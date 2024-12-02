using System.Collections.Generic;

namespace SpaceDeck.Tokenization.Minimum.Questions
{
    public class ExecutionAnswerSet
    {
        private readonly Dictionary<ExecutionQuestion, ExecutionAnswer> questionsToAnswers = new Dictionary<ExecutionQuestion, ExecutionAnswer>();

        public ExecutionAnswerSet(ExecutionQuestion question, ExecutionAnswer answer)
        {
            this.questionsToAnswers.Add(question, answer);
        }

        public bool TryGetAnswerForQuestion(ExecutionQuestion question, out ExecutionAnswer associatedAnswer)
        {
            return this.questionsToAnswers.TryGetValue(question, out associatedAnswer);
        }

        public bool TryGetTypedAnswerForQuestion<T>(ExecutionQuestion question, out ExecutionAnswer<T> associatedAnswer) where T : ExecutionQuestion
        {
            if (!this.TryGetAnswerForQuestion(question, out ExecutionAnswer unboxedAssociatedAnswer))
            {
                associatedAnswer = null;
                return false;
            }

            associatedAnswer = unboxedAssociatedAnswer as ExecutionAnswer<T>;
            return associatedAnswer != null;
        }
    }
}