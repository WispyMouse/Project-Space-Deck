namespace SpaceDeck.GameState.Changes.Targets
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using SpaceDeck.GameState.Minimum;

    /// <summary>
    /// Indicates that this change "targets" "nothing".
    /// There's nothing specific this ability should affect.
    /// Whatever the ability using this target is in relation to should
    /// be able to be executed without being provided the context of
    /// any particular targeting information.
    /// 
    /// This is a contextual evaluatable target, and only requires one instance of it due to its nature.
    /// You cannot initialize new instances, so you must access it through <see cref="Instance"/>.
    /// </summary>
    public class NobodyTarget : IChangeTarget
    {
        public readonly static NobodyTarget Instance = new NobodyTarget();

        private NobodyTarget()
        {

        }

        public IEnumerable<Entity> GetRepresentedEntities(IGameStateMutator gameState)
        {
            return Array.Empty<Entity>();
        }
    }
}