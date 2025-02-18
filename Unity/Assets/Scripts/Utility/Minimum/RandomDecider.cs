namespace SpaceDeck.Utility.Minimum
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class RandomDecider<T>
    {
        public T ChooseRandomly(IReadOnlyList<T> toChooseFrom)
        {
            if (toChooseFrom.Count == 0)
            {
                // TODO: LOG
                return default(T);
            }

            toChooseFrom = this.EliminateOptions(toChooseFrom);

            if (toChooseFrom.Count == 0)
            {
                // TODO: LOG
                return default(T);
            }

            T chosen = this.ChooseOneRandomly(toChooseFrom);
            this.NoteAsChosen(chosen);
            return chosen;
        }

        protected virtual IReadOnlyList<T> EliminateOptions(IReadOnlyList<T> fromList)
        {
            return fromList;
        }

        protected virtual T ChooseOneRandomly(IReadOnlyList<T> fromList)
        {
            int randomIndex = new Random().Next(0, fromList.Count);

            return fromList[randomIndex];
        }

        protected virtual void NoteAsChosen(T chosen)
        {

        }
    }
}