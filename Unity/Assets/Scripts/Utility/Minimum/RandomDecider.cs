namespace SpaceDeck.Utility.Minimum
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class RandomDecider<T>
    {
        public T ChooseRandomly(List<T> toChooseFrom)
        {
            if (toChooseFrom.Count == 0)
            {
                // TODO: LOG
                return default(T);
            }

            this.EliminateOptions(toChooseFrom);

            if (toChooseFrom.Count == 0)
            {
                // TODO: LOG
                return default(T);
            }

            T chosen = this.ChooseOneRandomly(toChooseFrom);
            this.NoteAsChosen(chosen);
            return chosen;
        }

        protected virtual void EliminateOptions(List<T> fromList)
        {

        }

        protected virtual T ChooseOneRandomly(List<T> fromList)
        {
            int randomIndex = new Random().Next(0, fromList.Count);
            return fromList[randomIndex];
        }

        protected virtual void NoteAsChosen(T chosen)
        {

        }
    }
}