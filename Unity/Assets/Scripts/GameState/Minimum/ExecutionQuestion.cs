namespace SpaceDeck.GameState.Minimum
{
    public abstract class ExecutionQuestion
    {
        public virtual bool TryGetDefaultAnswer(QuestionAnsweringContext answeringContext, out ExecutionAnswer answer)
        {
            answer = null;
            return false;
        }

        public virtual void ApplyDefaultToContext(QuestionAnsweringContext answeringContext)
        {
            // No-op
        }
    }

    /// <summary>
    /// Represents a question that must be answered to execute a <see cref="LinkedTokenList"/>.
    /// </summary>
    public abstract class ExecutionQuestion<A> : ExecutionQuestion where A : ExecutionAnswer
    {
        public override bool TryGetDefaultAnswer(QuestionAnsweringContext answeringContext, out ExecutionAnswer answer)
        {
            if (this.TryGetDefaultTypedAnswer(answeringContext, out A typedAnswer))
            {
                answer = typedAnswer;
                return true;
            }

            answer = null;
            return false;
        }

        public virtual bool TryGetDefaultTypedAnswer(QuestionAnsweringContext answeringContext, out A answer)
        {
            answer = null;
            return false;
        }
    }
}
