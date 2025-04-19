namespace SpaceDeck.GameState.Minimum
{
    using SpaceDeck.GameState.Minimum;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a running tally of information while starting execution.
    /// This can store things like "default targets" and "owner" in a way
    /// that can be used to help with answering questions.
    /// </summary>
    public class QuestionAnsweringContext
    {
        public Entity User;
        public IChangeTarget DefaultTarget;
        public readonly IGameStateMutator StartingGameState;
        public readonly CardInstance PlayingCard;

        public QuestionAnsweringContext(IGameStateMutator startingGameState, Entity user)
        {
            this.StartingGameState = startingGameState;
            this.User = user;
        }

        public QuestionAnsweringContext(IGameStateMutator startingGameState, Entity user, CardInstance playingCard) : this(startingGameState, user)
        {
            this.PlayingCard = playingCard;
        }
    }
}