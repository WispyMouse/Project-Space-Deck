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
        public readonly LinkedTokenList TokenList;
        public readonly ExecutionAnswerSet Answers;
        public readonly CardInstance PlayingCard;
        public readonly AppliedStatusEffect BasedOnAppliedStatusEffect;

        public Entity User;
        public IChangeTarget CurrentDefaultTarget;
        public GameStateChange CurrentlyExecutingGameStateChange;

        public ScriptingExecutionContext(IGameStateMutator executedOnGameState, LinkedTokenList tokenList, ExecutionAnswerSet answers)
        {
            this.ExecutedOnGameState = executedOnGameState;
            this.TokenList = tokenList;
            this.User = answers?.User;
            this.Answers = answers;
        }

        public ScriptingExecutionContext(IGameStateMutator executedOnGameState, LinkedTokenList tokenList, ExecutionAnswerSet answers, CardInstance playingCard) : this(executedOnGameState, tokenList, answers)
        {
            this.PlayingCard = playingCard;
        }

        public ScriptingExecutionContext(IGameStateMutator executedOnGameState, LinkedTokenList tokenList, ExecutionAnswerSet answers, AppliedStatusEffect statusEffect) : this(executedOnGameState, tokenList, answers)
        {
            this.BasedOnAppliedStatusEffect = statusEffect;
        }
    }
}