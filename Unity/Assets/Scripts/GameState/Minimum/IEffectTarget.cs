namespace SpaceDeck.GameState.Minimum
{
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// When an effect has a target, this interface can be used as the value type.
    /// Then the actual set of represented entities can be extracted from this,
    /// sometimes in a contextual way that would require the ability to currently
    /// be executing to be evaluated.
    /// </summary>
    public interface IChangeTarget
    {
        public IEnumerable<Entity> GetRepresentedEntities(IGameStateMutator gameState);
    }
}