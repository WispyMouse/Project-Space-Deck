namespace SpaceDeck.Tokenization.Minimum
{
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
        public LinkedToken(ScriptingCommand commandToExecute, List<LowercaseString> arguments) : base(commandToExecute, arguments)
        {
        }
    }
}