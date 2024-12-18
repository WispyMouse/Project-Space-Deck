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
    public class GameState
    {
        public EncounterState CurrentEncounterState = new EncounterState();
        public readonly List<Entity> PersistentEntities = new List<Entity>();

        public List<Entity> AllEntities
        {
            get
            {
                // TODO: This is pretty horrible (I wrote it!)! It has a lot of allocating the
                // same list repeatedly. We should revisit this soon.
                List<Entity> allEntities = new List<Entity>(PersistentEntities);
                allEntities.AddRange(this.CurrentEncounterState.EncounterEnemies);
                return allEntities;
            }
        }

        public GameState()
        {

        }
    }
}