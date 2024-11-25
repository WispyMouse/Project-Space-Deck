namespace SpaceDeck.GameState.Minimum
{
    using System.Collections;
    using System.Collections.Generic;

    public interface IChangeTarget
    {
        public IEnumerable<Entity> GetRepresentedEntities(ExecutionContext executionContext);
    }
}