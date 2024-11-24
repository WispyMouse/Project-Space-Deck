namespace SpaceDeck.Tokenization.ScriptingCommands
{
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.GameState.Changes.Targets;
    using SpaceDeck.Tokenization.Minimum.Evaluatables;
    using SpaceDeck.Tokenization.Processing;

    public class DamageScriptingCommand : ScriptingCommand
    {
        public override LowercaseString Identifier => "DAMAGE";

        public override bool TryGetLinkedToken(ParsedToken parsedToken, out LinkedToken linkedToken)
        {
            IChangeTarget target = PreviousTarget.Instance;

            // Presently, Damage Scripting Commands can only have one argument; the damage amount to deal
            // TODO: Add a target, so that the target can be set as an argument
            // TODO: Add a user, so that both the user and the target can be set as an argument
            if (parsedToken.Arguments.Count == 1)
            {
                // Try to evaluate the first token as a numeric value. If it can't be done, this isn't a hit.
                if (!EvaluatablesReference.TryGetNumericEvaluatableValue(parsedToken.Arguments[0], out INumericEvaluatableValue evaluatable))
                {
                    linkedToken = new DamageLinkedToken(parsedToken, target, evaluatable);
                    return true;
                }
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
        public readonly INumericEvaluatableValue Mod;

        public DamageLinkedToken(ParsedToken parsedToken, IChangeTarget changeTarget, int mod) : base(parsedToken)
        {
            this.ChangeTarget = changeTarget;
            this.Mod = new ConstantNumericValue(mod);
        }

        public DamageLinkedToken(ParsedToken parsedToken, IChangeTarget changeTarget, decimal mod) : base(parsedToken)
        {
            this.ChangeTarget = changeTarget;
            this.Mod = new ConstantNumericValue(mod);
        }

        public DamageLinkedToken(ParsedToken parsedToken, IChangeTarget changeTarget, INumericEvaluatableValue mod) : base(parsedToken)
        {
            this.ChangeTarget = changeTarget;
            this.Mod = mod;
        }
    }
}
