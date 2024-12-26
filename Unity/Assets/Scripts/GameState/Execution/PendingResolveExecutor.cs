namespace SpaceDeck.GameState.Execution
{
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Tokenization.Minimum.Context;
    using System.Collections;
    using System.Collections.Generic;
    using SpaceDeck.Utility.Minimum;


    public static class PendingResolveExecutor
    {
        public static void ResolveAll(IGameStateMutator mutator)
        {
            while (mutator.TryGetNextResolve(out IResolve next))
            {
                next.Apply(mutator);
            }
        }
    }
}