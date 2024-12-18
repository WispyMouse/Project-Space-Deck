namespace SpaceDeck.GameState.Rules
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using SpaceDeck.GameState.Changes;
    using SpaceDeck.GameState.Execution;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Tokenization.Minimum.Context;

    public class ZeroHealthRule : Rule
    {
        public override bool TryApplyRule(ScriptingExecutionContext context, IGameStateMutator gameStateMutator, GameStateChange change, out List<GameStateChange> applications)
        {
            applications = new List<GameStateChange>();

            foreach (Entity curEntity in context.ExecutedOnGameState.AllEntities)
            {
                if (gameStateMutator.EntityIsAlive(curEntity) && gameStateMutator.GetQuality(curEntity, "health", 0) <= 0)
                {
                    applications.Add(new RemoveEntity(curEntity));
                }
            }

            return applications.Count > 0;
        }
    }
}