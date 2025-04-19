namespace SpaceDeck.Tokenization.Minimum
{
    using SpaceDeck.GameState.Minimum;
    using System.Collections.Generic;
    using SpaceDeck.Tokenization.Minimum.Context;

    public abstract class LinkedExecutionQuestion<T> : ExecutionQuestion<T> where T : ExecutionAnswer
    {
        public readonly LinkedToken Token;

        public LinkedExecutionQuestion(LinkedToken linkedToken)
        {
            this.Token = linkedToken;
        }
    }
}