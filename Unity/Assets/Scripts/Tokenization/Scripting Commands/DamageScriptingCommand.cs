namespace SpaceDeck.Tokenization.ScriptingCommands
{
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.GameState.Changes.Targets;
    using SpaceDeck.Tokenization.Minimum.Evaluatables;
    using SpaceDeck.Tokenization.Processing;
    using SpaceDeck.GameState.Changes;

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
                    linkedToken = null;
                    return false;
                }

                linkedToken = new DamageLinkedToken(parsedToken, target, evaluatable);
                return true;
            }
            else
            {
                // If there aren't one or two arugments, it can't be a damage scripting command.
                linkedToken = null;
                return false;
            }

            linkedToken = null;
            return false;
        }

        public override bool TryApplyDelta(ExecutionContext executionContext, GameState stateToApplyTo, LinkedToken token, ref GameStateDelta delta)
        {
            if (!(token is DamageLinkedToken damageLinkedToken))
            {
                return false;
            }

            delta.Changes.Add(new ModifyQuality(damageLinkedToken.ChangeTarget, "health", damageLinkedToken.Mod));

            return true;
        }
    }

    public class DamageLinkedToken : LinkedToken<DamageScriptingCommand>
    {
        public readonly IChangeTarget ChangeTarget;
        public readonly INumericEvaluatableValue Mod;

        /// <param name="damageToApply">Amount of damage to apply. The negative of this value is taken.</param>
        public DamageLinkedToken(ParsedToken parsedToken, IChangeTarget changeTarget, int damageToApply) : base(parsedToken)
        {
            this.ChangeTarget = changeTarget;
            this.Mod = new ConstantNumericValue(-damageToApply);
        }


        /// <param name="damageToApply">Amount of damage to apply. The negative of this value is taken.</param>
        public DamageLinkedToken(ParsedToken parsedToken, IChangeTarget changeTarget, decimal damageToApply) : base(parsedToken)
        {
            this.ChangeTarget = changeTarget;
            this.Mod = new ConstantNumericValue(damageToApply);
        }
        
        /// <param name="mod">Amount of damage to apply. Is already assumed to be negative.</param>
        public DamageLinkedToken(ParsedToken parsedToken, IChangeTarget changeTarget, INumericEvaluatableValue mod) : base(parsedToken)
        {
            this.ChangeTarget = changeTarget;
            this.Mod = mod;
        }
    }
}
