namespace SpaceDeck.Tokenization.Minimum.Context
{
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.Tokenization.Minimum.Questions;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Represents the information about a currently executing <see cref="IGameStateMutator"/>.
    /// This holds information as a Delta is being applied, such as the previously established
    /// target, the user of the ability, etc.
    /// 
    /// This class changes as the set is executed, and should be maintained only during
    /// the lifetime of an execution.
    /// </summary>
    public class ScriptingExecutionContext
    {
        public readonly IGameStateMutator ExecutedOnGameState;
        public IChangeTarget CurrentDefaultTarget;
        public readonly LinkedTokenList TokenList;
        public readonly ExecutionAnswerSet Answers;
        public readonly CardInstance PlayingCard;

        public ScriptingExecutionContext(IGameStateMutator executedOnGameState, LinkedTokenList tokenList, ExecutionAnswerSet answers, CardInstance playingCard)
        {
            this.ExecutedOnGameState = executedOnGameState;
            this.TokenList = tokenList;
            this.Answers = answers;
            this.PlayingCard = playingCard;
        }
    }
}