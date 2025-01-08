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
    using SpaceDeck.Utility.Minimum;

    public class TargetScriptingCommand : ScriptingCommand
    {
        public override LowercaseString Identifier => "TARGET";

        public override bool TryGetLinkedToken(ParsedToken parsedToken, out LinkedToken linkedToken)
        {
            // Presently, Target Scripting Commands have one "target" text
            if (parsedToken.Arguments.Count != 1)
            {
                linkedToken = null;
                return false;
            }

            // Try to evaluate the first token as a numeric value. If it can't be done, this isn't a hit.
            if (!EvaluatablesReference.TryGetChangeTargetEvaluatableValue(parsedToken.Arguments[0], out ChangeTargetEvaluatableValue evaluatable))
            {
                linkedToken = null;
                return false;
            }

            linkedToken = new TargetLinkedToken(evaluatable, parsedToken);
            return true;
        }
    }

    public class TargetLinkedToken : LinkedToken<TargetScriptingCommand>
    {
        public readonly ChangeTargetEvaluatableValue ChangeTarget;
        public readonly INumericEvaluatableValue Mod;

        public TargetLinkedToken(ChangeTargetEvaluatableValue changeTarget, ParsedToken parsedToken) : base(parsedToken)
        {
            this.ChangeTarget = changeTarget;
            this._Questions.AddRange(ChangeTarget.GetQuestions(this));
        }

        public override bool TryGetChanges(ScriptingExecutionContext context, out Stack<GameStateChange> changes)
        {
            if (!this.ChangeTarget.TryEvaluate(context, out IChangeTarget target))
            {
                changes = null;
                return false;
            }

            context.CurrentDefaultTarget = target;
            changes = null;
            return true;
        }
    }
}
