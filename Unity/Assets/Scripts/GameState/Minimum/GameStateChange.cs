namespace SpaceDeck.GameState.Minimum
{
    using SpaceDeck.Utility.Minimum;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Represents anything that affects a <see cref="GameState"/>
    /// in any way. This is the base class of all potential changes.
    /// </summary>
    public abstract class GameStateChange : IResolve, IDescribable
    {
        public readonly IChangeTarget Target;
        public bool Triggered = false;

        public virtual bool ShouldKeepHistory => true;

        public GameStateChange(IChangeTarget target)
        {
            this.Target = target;
        }

        public abstract void Apply(IGameStateMutator toApplyTo);

        public virtual void Trigger(IGameStateMutator toPushTriggers)
        {
            // The only way a GameStateChange can be properly observed on triggering is if it is called out
            // Otherwise, it is assumed nothing can react to the event.
            // Just push this as a resolve.
            toPushTriggers.PushResolve(this);
        }

        public abstract string Describe();

        public static string Describe(IEnumerable<GameStateChange> changes)
        {
            StringBuilder changeString = new StringBuilder();

            string prependString = "";
            foreach (GameStateChange change in changes)
            {
                string thisChangeText = change.Describe();

                if (!string.IsNullOrEmpty(thisChangeText))
                {
                    changeString.Append(prependString + thisChangeText);
                    prependString = " ";
                }
            }

            return changeString.ToString();
        }
    }
}