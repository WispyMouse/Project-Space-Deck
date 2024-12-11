namespace SpaceDeck.Tokenization.Minimum
{
    using SpaceDeck.Tokenization.Minimum.Questions;
    using SpaceDeck.GameState.Minimum;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using SpaceDeck.Tokenization.Minimum.Context;

    /// <summary>
    /// The combination of <see cref="ParsedToken"/>,
    /// database access, and saturation of arguments.
    /// 
    /// This is a mostly processed and prepared to be Executed token.
    /// </summary>
    public class LinkedToken : ParsedToken
    {
        public LinkedTokenScope LinkedScope;
        public LinkedToken NextLinkedToken;

        public IReadOnlyList<ExecutionQuestion> Questions
        {
            get
            {
                return this._Questions;
            }
        }
        protected readonly List<ExecutionQuestion> _Questions = new List<ExecutionQuestion>();

        public LinkedToken(ParsedToken parsedToken) : base(parsedToken.CommandToExecute, parsedToken.Arguments)
        {
        }

        public virtual bool TryGetChanges(ExecutionContext context, out List<GameStateChange> changes)
        {
            changes = null;
            return true;
        }
    }

    /// <summary>
    /// This represents that this <see cref="LinkedToken"/> is made to represent
    /// the result of a <see cref="ScriptingCommand"/> hydrating itself with its
    /// arguments.
    /// </summary>
    /// <typeparam name="T">The type of ScriptingCommand this represents.</typeparam>
    public abstract class LinkedToken<T> : LinkedToken where T : ScriptingCommand
    {
        public LinkedToken(ParsedToken parsedToken) : base(parsedToken)
        {
        }
    }
}