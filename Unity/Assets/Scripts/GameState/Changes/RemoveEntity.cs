namespace SpaceDeck.GameState.Changes
{
    using System.Collections;
    using System.Collections.Generic;
    using SpaceDeck.GameState.Minimum;

    public class RemoveEntity : GameStateChange
    {
        public RemoveEntity(IChangeTarget target) : base(target)
        {
        }

        public override void ApplyToGameState(ref GameState toApplyTo)
        {
            foreach (Entity curEntity in new List<Entity>(this.Target.GetRepresentedEntities()))
            {
                toApplyTo.CurrentEncounterState.EncounterEnemies.Remove(curEntity);
                curEntity.IsAlive = false;
            }
        }
    }
}