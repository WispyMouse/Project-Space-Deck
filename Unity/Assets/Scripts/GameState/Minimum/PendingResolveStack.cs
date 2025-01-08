namespace SpaceDeck.GameState.Minimum
{
    using SpaceDeck.Utility.Minimum;
    using System.Collections.Generic;

    /// <summary>
    /// Describes an organized holder of pending <see cref="IResolve"/> events.
    /// This helps organize the execution of events, similar to a stack.
    /// </summary>
    public class PendingResolveStack
    {
        private readonly Stack<IResolve> PendingResolves = new Stack<IResolve>();
        public IResolve CurrentlyResolving { get; private set; } = null;

        public void Push(IResolve toPush)
        {
            this.PendingResolves.Push(toPush);
        }

        public bool TryGetNextResolve(out IResolve next)
        {
            if (PendingResolves.Count == 0)
            {
                next = null;
                return false;
            }

            next = this.PendingResolves.Pop();
            CurrentlyResolving = next;
            return true;
        }

        public void ClearCurrentlyResolving()
        {
            this.CurrentlyResolving = null;
        }
    }
}