namespace SpaceDeck.Tokenization.Minimum.Questions
{
    public struct LinkedExecutionAnswer
    {
        public LinkedToken AssociatedToken;
        public ExecutionQuestion Question;
        public ExecutionAnswer Answer;

        public LinkedExecutionAnswer(LinkedToken associatedToken, ExecutionQuestion question, ExecutionAnswer answer)
        {
            this.AssociatedToken = associatedToken;
            this.Question = question;
            this.Answer = answer;
        }
    }
}