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

    public class ReduceIntensityScriptingCommand : ScriptingCommand
    {
        public override LowercaseString Identifier => "REDUCE";

        public override bool TryGetLinkedToken(ParsedToken parsedToken, out LinkedToken linkedToken)
        {
            if (parsedToken.Arguments.Count != 1)
            {
                linkedToken = null;
                return false;
            }

            if (!EvaluatablesReference.TryGetNumericEvaluatableValue(parsedToken.Arguments[0], out INumericEvaluatableValue reduceIntensity))
            {
                linkedToken = null;
                return false;
            }

            linkedToken = new ReduceIntensityLinkedToken(parsedToken, reduceIntensity);
            return true;
        }
    }

    public class ReduceIntensityLinkedToken : LinkedToken<ReduceIntensityScriptingCommand>
    {
        public readonly INumericEvaluatableValue ReduceBy;

        public ReduceIntensityLinkedToken(ParsedToken parsedToken, INumericEvaluatableValue reduceIntensity) : base(parsedToken)
        {
            this.ReduceBy = reduceIntensity;
            this._Questions.AddRange(this.ReduceBy.GetQuestions(this));
        }

        public override bool TryGetChanges(ScriptingExecutionContext context, out Stack<GameStateChange> changes)
        {
            if (context.CurrentlyExecutingGameStateChange == null)
            {
                changes = null;
                return false;
            }

            if (!this.ReduceBy.TryEvaluate(context, out decimal intensity))
            {
                changes = null;
                return false;
            }

            changes = new Stack<GameStateChange>();
            changes.Push(new ModifyIntensity(context.CurrentlyExecutingGameStateChange, intensity, InitialIntensityPositivity.PositiveOrZero));

            return true;
        }
    }
}
