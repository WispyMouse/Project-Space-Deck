namespace SpaceDeck.Tokenization.Minimum.Questions
{
    /// <summary>
    /// Represents a question that must be answered to execute a <see cref="LinkedTokenList"/>.
    /// </summary>
    public abstract class ExecutionQuestion
    {
        public readonly LinkedToken Token;

        public ExecutionQuestion(LinkedToken token)
        {
            this.Token = token;
        }
    }
}
