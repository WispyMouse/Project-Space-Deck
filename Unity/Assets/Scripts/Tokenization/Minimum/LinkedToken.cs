namespace SpaceDeck.Tokenization.Minimum
{
    using SpaceDeck.Tokenization.Minimum.Questions;
    using System;
    using System.Collections;
    using System.Collections.Generic;

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

        public virtual IEnumerable<ExecutionQuestion> Questions => Array.Empty<ExecutionQuestion>();

        public LinkedToken(ParsedToken parsedToken) : base(parsedToken.CommandToExecute, parsedToken.Arguments)
        {
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