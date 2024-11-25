namespace SpaceDeck.GameState.Minimum
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Describes the state of a game.
    /// 
    /// This is meant to be entirely hollistic, holding information
    /// about every character, decision, card, deck, battle, etc.
    /// 
    /// This is not necessarily the current game's state. This object
    /// can be cloned, allowing for theoretical branches of the game's
    /// state in order to show certain properties and values that might
    /// exist after something is executed.
    /// </summary>
    public class GameState : ICloneable
    {
        public EncounterState CurrentEncounterState;
        public readonly List<Entity> PersistentEntities = new List<Entity>();

        public GameState()
        {

        }

        /// <summary>
        /// Construtor used to clone another GameState.
        /// This is a deep copy; do not share any references.
        /// </summary>
        /// <param name="toClone">The GameState to clone.</param>
        private GameState(GameState toClone)
        {
            if (toClone.CurrentEncounterState != null)
            {
                // TODO: CLONE CURRENTENCOUNTERSTATE
            }
            
            foreach(Entity cloningEntity in toClone.PersistentEntities)
            {
                // TODO: CLONE PERSISTENTENTITIES
            }
        }

        public object Clone()
        {
            GameState clonedState = new GameState(this);
            return clonedState;
        }

        public GameState GetClone() => this.Clone() as GameState;

    }
}