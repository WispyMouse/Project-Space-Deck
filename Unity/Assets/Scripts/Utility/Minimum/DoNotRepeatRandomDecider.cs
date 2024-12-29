namespace SpaceDeck.Utility.Minimum
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class DoNotRepeatRandomDecider<T> : RandomDecider<T>
    {
        public HashSet<T> AlreadyChosen = new HashSet<T>();

        protected override void EliminateOptions(List<T> fromList)
        {
            base.EliminateOptions(fromList);

            List<T> originalList = new List<T>(fromList);

            foreach (T item in this.AlreadyChosen)
            {
                fromList.Remove(item);
            }

            // If we've exhausted the options, reset the chosen list
            if (fromList.Count == 0)
            {
                fromList.AddRange(originalList);
                this.AlreadyChosen.Clear();
            }
        }

        protected override void NoteAsChosen(T chosen)
        {
            base.NoteAsChosen(chosen);

            this.AlreadyChosen.Add(chosen);
        }
    }
}
