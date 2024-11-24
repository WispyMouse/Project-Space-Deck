namespace SpaceDeck.Tokenization.ScriptingCommands
{
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.GameState.Changes.Targets;

    public class DamageScriptingCommand : ScriptingCommand
    {
        public override LowercaseString Identifier => "DAMAGE";

        public bool TryGetLinkedToken(ParsedToken parsedToken, out LinkedToken linkedToken)
        {
            IChangeTarget target = PreviousTarget.Instance;

            // Damage scripts require exactly one or two arguments.
            // If there are two arguments, one must be an evaluatable number, and the other must be an evaluatable target.
            // If there is one argument, it must be an evaluatable number. The target is assumed to be the previous target.
            // that was specific in the executing parse tree.
            if (parsedToken.Arguments.Count == 1)
            {

            }
            else if (parsedToken.Arguments.Count == 2)
            {

            }
            else
            {
                // If there aren't one or two arugments, it can't be a damage scripting command.
                linkedToken = null;
                return false;
            }

            linkedToken = new LinkedToken(parsedToken);
            return true;
        }

        public override bool TryApplyDelta(GameState stateToApplyTo, LinkedToken token, ref GameStateDelta delta)
        {
            return base.TryApplyDelta(stateToApplyTo, token, ref delta);
        }
    }

    public class DamageLinkedToken : LinkedToken<DamageScriptingCommand>
    {
        public readonly IChangeTarget ChangeTarget;
        public readonly int Mod;

        public DamageLinkedToken(ParsedToken parsedToken, IChangeTarget changeTarget, int mod) : base(parsedToken)
        {
            this.ChangeTarget = changeTarget;
            this.Mod = mod;
        }
    }
}
