namespace SpaceDeck.GameState.Execution
{
    using SpaceDeck.Tokenization.Minimum;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a token set with the appropriate play context
    /// to start representing the necessary questions to answer
    /// in order to play the card.
    /// 
    /// When this is provided with answers, it can be parsed
    /// into a <see cref="GameState.Minimum.GameStateDelta"/>.
    /// </summary>
    public struct ContextualizedTokenSet
    {
        public readonly LinkedTokenSet Tokens;

        public ContextualizedTokenSet(LinkedTokenSet tokens)
        {
            this.Tokens = tokens;
        }
    }
}