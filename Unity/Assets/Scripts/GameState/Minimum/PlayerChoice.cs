namespace SpaceDeck.GameState.Minimum
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using SpaceDeck.GameState.Minimum;

    public abstract class PlayerChoice
    {
        public bool ResultIsChosen { get; protected set; } = false;
        public abstract string _DescribeChoice(IGameStateMutator mutator);
        public abstract bool _TryFinalizeWithoutPlayerInput(IGameStateMutator mutator);
    }

    public abstract class PlayerChoice<T> : PlayerChoice
    {
        public T ChosenResult { get; private set; }
        public virtual void _SetChoice(IGameStateMutator mutator, T result)
        {
            this.ChosenResult = result;
            this.ResultIsChosen = true;
        }
    }
}