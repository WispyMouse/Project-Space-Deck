namespace SpaceDeck.Utility.Minimum
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class DoNotRepeatRandomDecider<T> : RandomDecider<T>
    {
        public HashSet<T> AlreadyChosen = new HashSet<T>();

        protected override IReadOnlyList<T> EliminateOptions(IReadOnlyList<T> originalFromList)
        {
            List<T> workingFromList = new List<T>(base.EliminateOptions(originalFromList));

            foreach (T item in this.AlreadyChosen)
            {
                workingFromList.Remove(item);
            }

            // If we've exhausted the options, reset the chosen list
            if (workingFromList.Count == 0)
            {
                this.AlreadyChosen.Clear();
                return originalFromList;
            }

            return workingFromList;
        }

        protected override void NoteAsChosen(T chosen)
        {
            base.NoteAsChosen(chosen);

            this.AlreadyChosen.Add(chosen);
        }
    }
}
