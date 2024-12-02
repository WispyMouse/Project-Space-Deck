namespace SpaceDeck.GameState.Context
{
    using SpaceDeck.GameState.Minimum;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Represents the information about a currently executing <see cref="GameStateDelta"/>.
    /// This holds information as a Delta is being applied, such as the previously established
    /// target, the user of the ability, etc.
    /// 
    /// This class changes as the set is executed, and should be maintained only during
    /// the lifetime of an execution.
    /// </summary>
    public class ExecutionContext
    {
        public IChangeTarget CurrentDefaultTarget;
        public ContextualizedTokenList TokenList;
    }
}