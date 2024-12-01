namespace SpaceDeck.Tokenization.Minimum.Questions
{
    public struct LinkedExecutionQuestion
    {
        public LinkedToken AssociatedToken;
        public ExecutionQuestion Question;

        public LinkedExecutionQuestion(LinkedToken associatedToken, ExecutionQuestion question)
        {
            this.AssociatedToken = associatedToken;
            this.Question = question;
        }
    }
}