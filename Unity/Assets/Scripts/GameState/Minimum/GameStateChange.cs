namespace SpaceDeck.GameState.Minimum
{
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Represents anything that affects a <see cref="GameState"/>
    /// in any way. This is the base class of all potential changes.
    /// </summary>
    public abstract class GameStateChange
    {
        public readonly IChangeTarget Target;

        public GameStateChange(IChangeTarget target)
        {
            this.Target = target;
        }

        public abstract void ApplyToGameState(IGameStateMutator toApplyTo);
    }
}