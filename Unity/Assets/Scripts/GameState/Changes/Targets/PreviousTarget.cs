namespace SpaceDeck.GameState.Changes.Targets
{
    using System.Collections;
    using SpaceDeck.GameState.Minimum;
    
    /// <summary>
    /// Indicates that this ability should use the target that
    /// this effect has been using.
    /// 
    /// This will fail to evaluate and should result in an error if a parse tree has been
    /// built such that there is a "null" previous target.
    /// 
    /// This is the default assumed targets for most ScriptingCommands.
    /// If a ScriptingCommand doesn't provide a target, this is usually
    /// what will be used.
    /// 
    /// It is possible that the target provided will change the
    /// effect of abilities, such as failing to be possible to
    /// execute with certain targets.
    /// 
    /// This is a contextual evaluatable target, and only requires one instance of it due to its nature.
    /// You cannot initialize new instances, so you must access it through <see cref="Instance"/>.
    /// </summary>
    public class PreviousTarget : IChangeTarget
    {
        public readonly static PreviousTarget Instance = new PreviousTarget();

        private PreviousTarget()
        {

        }
    }
}