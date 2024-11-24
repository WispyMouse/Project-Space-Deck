namespace SpaceDeck.GameState.Minimum
{
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Describes the state of a encounter.
    /// 
    /// This is meant to describe every aspect of an encounter, from
    /// cards in hand to enemies involved and their statistics. This
    /// is scoped to a single Encounter, exposing some configuration
    /// not possible in <see cref="GameState"/>. A Gamestate optionally
    /// holds an EncounterState.
    /// 
    /// This class is meant to be cloneable, allowing for theoretical
    /// branching of battle states.
    /// </summary>
    public class EncounterState
    {
        public readonly List<Entity> EncounterEnemies = new List<Entity>();
    }
}