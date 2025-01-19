namespace SpaceDeck.Tokenization.ScriptingCommands
{
    using System;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.Tokenization.Evaluatables;
    using SpaceDeck.Tokenization.Processing;
    using SpaceDeck.GameState.Changes;
    using System.Collections.Generic;
    using SpaceDeck.Tokenization.Minimum.Questions;
    using SpaceDeck.Tokenization.Minimum.Context;
    using SpaceDeck.Tokenization.Evaluatables.Questions;
    using SpaceDeck.Utility.Minimum;
    using SpaceDeck.Utility.Wellknown;
    using SpaceDeck.Utility.Logging;

    public class DamageScriptingCommand : ScriptingCommand
    {
        public override LowercaseString Identifier => "DAMAGE";

        public override bool TryGetLinkedToken(ParsedToken parsedToken, out LinkedToken linkedToken)
        {
            // Presently, Damage Scripting Commands can only have one argument; the damage amount to deal
            // TODO: Add a target, so that the target can be set as an argument
            // TODO: Add a user, so that both the user and the target can be set as an argument
            if (parsedToken.Arguments.Count != 1)
            {
                Logging.DebugLog(WellknownLoggingLevels.Error,
                    WellknownLoggingCategories.GetLinkedScriptingToken,
                    $"Damage token requires exactly one argument. Number of arguments: {parsedToken.Arguments.Count}");
                linkedToken = null;
                return false;
            }

            // Try to evaluate the first token as a numeric value. If it can't be done, this isn't a hit.
            if (!EvaluatablesReference.TryGetNumericEvaluatableValue(parsedToken.Arguments[0], out INumericEvaluatableValue evaluatable))
            {
                Logging.DebugLog(WellknownLoggingLevels.Error,
                    WellknownLoggingCategories.GetLinkedScriptingToken,
                    $"Failed to parse numeric evaluatalbe argument out of one argument. Argument: '{parsedToken.Arguments[0]}'");
                linkedToken = null;
                return false;
            }

            linkedToken = new DamageLinkedToken(parsedToken, new ChangeTargetEvaluatableValue(DefaultTargetProvider.Instance), evaluatable);
            return true;
        }
    }

    public class DamageLinkedToken : LinkedToken<DamageScriptingCommand>
    {
        public readonly ChangeTargetEvaluatableValue ChangeTarget;
        public readonly INumericEvaluatableValue Mod;

        public DamageLinkedToken(ChangeTargetEvaluatableValue changeTarget, ParsedToken parsedToken, INumericEvaluatableValue mod) : base(parsedToken)
        {
            this.ChangeTarget = changeTarget;
            this._Questions.AddRange(ChangeTarget.GetQuestions(this));
            this.Mod = mod;
        }


        /// <param name="damageToApply">Amount of damage to apply. The negative of this value is taken.</param>
        public DamageLinkedToken(ParsedToken parsedToken, ChangeTargetEvaluatableValue changeTarget, int damageToApply) : this(changeTarget, parsedToken, new ConstantNumericValue(-damageToApply))
        {
        }

        /// <param name="damageToApply">Amount of damage to apply. The negative of this value is taken.</param>
        public DamageLinkedToken(ParsedToken parsedToken, ChangeTargetEvaluatableValue changeTarget, decimal damageToApply) : this(changeTarget, parsedToken, new ConstantNumericValue(-damageToApply))
        {
        }

        /// <param name="mod">Amount of damage to apply. The negative of this value is taken.</param>
        public DamageLinkedToken(ParsedToken parsedToken, ChangeTargetEvaluatableValue changeTarget, INumericEvaluatableValue mod) : this(changeTarget, parsedToken,
            new NegativeNumericEvaluatableValue(mod))
        {
        }

        public override bool TryGetChanges(ScriptingExecutionContext context, out Stack<GameStateChange> changes)
        {
            if (!this.ChangeTarget.TryEvaluate(context, out IChangeTarget target) || !this.Mod.TryEvaluate(context, out decimal mod))
            {
                changes = null;
                return false;
            }

            changes = new Stack<GameStateChange>();
            foreach (Entity representedEntity in target.GetRepresentedEntities(context.ExecutedOnGameState))
            {
                changes.Push(new ModifyNumericQuality(representedEntity, representedEntity, WellknownQualities.Health, mod, InitialIntensityPositivity.NegativeOrZero));
            }
            return true;
        }
    }
}
