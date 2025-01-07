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

    public class ApplyStatusEffectStacksScriptingCommand : ScriptingCommand
    {
        public override LowercaseString Identifier => "APPLYSTATUSEFFECTSTACKS";

        public override bool TryGetLinkedToken(ParsedToken parsedToken, out LinkedToken linkedToken)
        {
            // Presently, Apply Status effect Stacks Scripting command needs exactly two arguments;
            // the status to apply, and the amount of stacks to apply
            // TODO: Add a target, so that the target can be set as an argument
            // TODO: Add a user, so that both the user and the target can be set as an argument
            if (parsedToken.Arguments.Count == 2)
            {
                // Try to evaluate the second token as a numeric value. If it can't be done, this isn't a hit.
                if (!EvaluatablesReference.TryGetNumericEvaluatableValue(parsedToken.Arguments[1], out INumericEvaluatableValue evaluatable))
                {
                    linkedToken = null;
                    return false;
                }

                linkedToken = new ApplyStatusEffectStacksLinkedToken(
                    new ChangeTargetEvaluatableValue(DefaultTargetProvider.Instance),
                    parsedToken,
                    parsedToken.Arguments[0],
                    evaluatable);
                return true;
            }
            else
            {
                // If there aren't one or two arugments, it can't be a damage scripting command.
                linkedToken = null;
                return false;
            }

            // linkedToken = null;
            // return false;
        }
    }

    public class ApplyStatusEffectStacksLinkedToken : LinkedToken<ApplyStatusEffectStacksScriptingCommand>
    {
        public readonly ChangeTargetEvaluatableValue ChangeTarget;
        public readonly LowercaseString StatusEffect;
        public readonly INumericEvaluatableValue Mod;

        public ApplyStatusEffectStacksLinkedToken(ChangeTargetEvaluatableValue changeTarget, ParsedToken parsedToken, LowercaseString statusEffect, INumericEvaluatableValue stacksToApply) : base(parsedToken)
        {
            this.ChangeTarget = changeTarget;
            this.Mod = stacksToApply;
            this.StatusEffect = statusEffect;
            this._Questions.AddRange(ChangeTarget.GetQuestions(this));
        }

        public ApplyStatusEffectStacksLinkedToken(ChangeTargetEvaluatableValue changeTarget, ParsedToken parsedToken, LowercaseString statusEffect, int stacksToApply) : this(changeTarget, parsedToken, statusEffect, new ConstantNumericValue(stacksToApply))
        {

        }

        public override bool TryGetChanges(ScriptingExecutionContext context, out List<GameStateChange> changes)
        {
            if (!this.ChangeTarget.TryEvaluate(context, out IChangeTarget target) || !this.Mod.TryEvaluate(context, out decimal mod))
            {
                changes = null;
                return false;
            }

            changes = new List<GameStateChange>() { new ModifyStatusEffectStacks(target, this.StatusEffect, mod) };
            return true;
        }
    }
}
