namespace SpaceDeck.GameState.Minimum
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using SpaceDeck.Utility.Minimum;

    /// <summary>
    /// Provides a low-scoped way to provide Elements,
    /// as opposed to direct access to ElementDatabase.
    /// </summary>
    public interface IElementProvider
    {
        bool TryGetElement(LowercaseString elementId, out Element foundElement);
    }
}