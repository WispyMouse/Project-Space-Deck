namespace SpaceDeck.Tokenization.Minimum.Questions
{
    public struct LinkedExecutionAnswer
    {
        public LinkedExecutionQuestion Question;
        public ExecutionAnswer Answer;

        public LinkedExecutionAnswer(LinkedExecutionQuestion question, ExecutionAnswer answer)
        {
            this.Question = question;
            this.Answer = answer;
        }
    }
}