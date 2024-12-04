namespace SpaceDeck.Tokenization.Minimum.Questions
{
    public abstract class ExecutionQuestion
    {
        public abstract ExecutionAnswer GetAnswerByIndex(int index);
    }

    /// <summary>
    /// Represents a question that must be answered to execute a <see cref="LinkedTokenList"/>.
    /// </summary>
    public abstract class ExecutionQuestion<A> : ExecutionQuestion where A : ExecutionAnswer
    {
        public readonly LinkedToken Token;

        public ExecutionQuestion(LinkedToken token)
        {
            this.Token = token;
        }

        public override ExecutionAnswer GetAnswerByIndex(int index)
        {
            return this.GetTypedAnswerByIndex(index);
        }

        public abstract A GetTypedAnswerByIndex(int index);
    }
}
