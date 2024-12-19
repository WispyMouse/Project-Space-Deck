using SpaceDeck.Tokenization.Minimum.Context;

namespace SpaceDeck.Tokenization.Minimum.Questions
{
    /// <summary>
    /// Represents an answer to a particular kind of <see cref="ExecutionQuestion"/>.
    /// </summary>
    public abstract class ExecutionAnswer
    {
        public ExecutionQuestion Question { get; set; }

        public ExecutionAnswer(ExecutionQuestion question)
        {
            this.Question = question;
        }

        public virtual void ApplyToQuestionAnsweringContext(QuestionAnsweringContext context)
        {

        }
    }

    /// <summary>
    /// Represents an answer to a particular kind of <see cref="ExecutionQuestion"/>.
    /// This one is linked to a particular kind of ExecutionQuestion.
    /// </summary>
    public abstract class ExecutionAnswer<T> : ExecutionAnswer where T : ExecutionQuestion
    {
        public T BoxedQuestion { get; set; }

        public ExecutionAnswer(ExecutionQuestion question) : base(question)
        {
        }
    }
}
