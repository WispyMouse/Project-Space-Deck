namespace SpaceDeck.GameState.Execution
{
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Tokenization.Minimum.Context;
    using System.Collections;
    using System.Collections.Generic;

    public class Rule
    {
        public virtual bool TryApplyRule(ScriptingExecutionContext context, GameStateChange change, out List<GameStateChange> applications)
        {
            applications = null;
            return false;
        }
    }
}