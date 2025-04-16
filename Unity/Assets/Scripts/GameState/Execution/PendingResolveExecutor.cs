namespace SpaceDeck.GameState.Execution
{
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Tokenization.Minimum.Context;
    using System.Collections;
    using System.Collections.Generic;
    using SpaceDeck.Utility.Minimum;
    using SpaceDeck.GameState.Deltas;

    public static class PendingResolveExecutor
    {
        public static void ResolveAll(IGameStateMutator mutator)
        {
            while (mutator.TryGetNextResolve(out IResolve next))
            {
                GameStateDeltaApplier.ApplyResolve(mutator, next);
            }
        }
    }
}