namespace SpaceDeck.Tokenization.Minimum.Context
{
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.Tokenization.Minimum.Questions;
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
        public IChangeTarget DefaultTarget;
        public readonly GameState StartingGameState;

        public QuestionAnsweringContext(GameState startingGameState)
        {
            this.StartingGameState = startingGameState;
        }
    }
}