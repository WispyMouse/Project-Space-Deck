namespace SpaceDeck.GameState.Minimum
{
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a list of <see cref="GameStateChange"/>.
    /// This can be used to represent what will happen when
    /// an entire effect resolves.
    /// </summary>
    public class GameStateDelta
    {
        public readonly List<GameStateChange> Changes = new List<GameStateChange>();
    }
}