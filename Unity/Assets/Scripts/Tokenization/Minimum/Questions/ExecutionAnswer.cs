namespace SpaceDeck.Tokenization.Minimum.Questions
{
    /// <summary>
    /// Represents an answer to a particular kind of <see cref="ExecutionQuestion"/>.
    /// </summary>
    public abstract class ExecutionAnswer
    {

    }

    /// <summary>
    /// Represents an answer to a particular kind of <see cref="ExecutionQuestion"/>.
    /// This one is linked to a particular kind of ExecutionQuestion.
    /// </summary>
    public abstract class ExecutionAnswer<T> : ExecutionQuestion where T : ExecutionQuestion
    {

    }
}
