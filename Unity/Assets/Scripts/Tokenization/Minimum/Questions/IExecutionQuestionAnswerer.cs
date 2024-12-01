namespace SpaceDeck.Tokenization.Minimum.Questions
{
    /// <summary>
    /// Provides a resource for answering <see cref="ExecutionQuestion"/>s.
    /// 
    /// This should be pretty vague about the actual user experience involved,
    /// such that these can be answered by testable implementations.
    /// </summary>
    public interface IExecutionQuestionAnswerer
    {
        bool TryStartAnswerQuestion(ExecutionQuestion question);
        ExecutionAnswer<T> ProvideAnswer<T>(T question, ExecutionAnswer<T> answer) where T : ExecutionQuestion;
    }
}