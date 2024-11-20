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
        public LinkedTokenScope LinkedScope;
        public LinkedToken NextLinkedToken;

        public LinkedToken(ParsedToken parsedToken) : base(parsedToken.CommandToExecute, parsedToken.Arguments)
        {
        }
    }
}