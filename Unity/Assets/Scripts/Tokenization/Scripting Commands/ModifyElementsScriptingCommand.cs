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
    using SpaceDeck.Models.Databases;
    using SpaceDeck.Utility.Logging;

    public class ModifyElementScriptingCommand : ScriptingCommand
    {
        public override LowercaseString Identifier => "MODIFYELEMENT";

        public override bool TryGetLinkedToken(ParsedToken parsedToken, out LinkedToken linkedToken)
        {
            if (parsedToken.Arguments.Count != 2)
            {
                linkedToken = null;
                return false;
            }

            if (!ElementDatabase.TryGetElement(parsedToken.Arguments[0], out Element matchedElement))
            {
                linkedToken = null;
                return false;
            }

            if (!EvaluatablesReference.TryGetNumericEvaluatableValue(parsedToken.Arguments[1], out INumericEvaluatableValue reduceIntensity))
            {
                linkedToken = null;
                return false;
            }

            linkedToken = new ModifyElementLinkedToken(parsedToken, matchedElement, reduceIntensity);
            return true;
        }
    }

    public class ModifyElementLinkedToken : LinkedToken<ModifyElementScriptingCommand>
    {
        public readonly Element ElementToModify;
        public readonly INumericEvaluatableValue ModIntensity;

        public ModifyElementLinkedToken(ParsedToken parsedToken, Element elementToModify, INumericEvaluatableValue modIntensity) : base(parsedToken)
        {
            this.ModIntensity = modIntensity;
            this.ElementToModify = elementToModify;
        }

        public override bool TryGetChanges(ScriptingExecutionContext context, out Stack<GameStateChange> changes)
        {
            if (!this.ModIntensity.TryEvaluate(context, out decimal intensity))
            {
                Logging.DebugLog(WellknownLoggingLevels.Error,
                    WellknownLoggingCategories.TokenTryGetChanges,
                    $"Could not evaluate {nameof(this.ModIntensity)}.");
                changes = null;
                return false;
            }

            changes = new Stack<GameStateChange>();
            changes.Push(new ModifyElement(this.ElementToModify, (int)intensity, InitialIntensityPositivity.PositiveOrZero));

            return true;
        }
    }
}
