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

    public class ModCurrencyScriptingCommand : ScriptingCommand
    {
        public override LowercaseString Identifier => "MODCURRENCY";

        public override bool TryGetLinkedToken(ParsedToken parsedToken, out LinkedToken linkedToken)
        {
            if (parsedToken.Arguments.Count != 2)
            {
                linkedToken = null;
                return false;
            }

            if (!CurrencyDatabase.TryGet(parsedToken.Arguments[0], out Currency matchedCurrency))
            {
                linkedToken = null;
                return false;
            }

            if (!EvaluatablesReference.TryGetNumericEvaluatableValue(parsedToken.Arguments[1], out INumericEvaluatableValue reduceIntensity))
            {
                linkedToken = null;
                return false;
            }

            linkedToken = new ModCurrencyLinkedToken(parsedToken, matchedCurrency, reduceIntensity);
            return true;
        }
    }

    public class ModCurrencyLinkedToken : LinkedToken<ModCurrencyScriptingCommand>
    {
        public readonly Currency CurrencyToMod;
        public readonly INumericEvaluatableValue ModIntensity;

        public ModCurrencyLinkedToken(ParsedToken parsedToken, Currency currencyToMod, INumericEvaluatableValue modIntensity) : base(parsedToken)
        {
            this.ModIntensity = modIntensity;
            this.CurrencyToMod = currencyToMod;
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
            changes.Push(new ModifyCurrency(this.CurrencyToMod, (int)intensity, InitialIntensityPositivity.PositiveOrZero));

            return true;
        }
    }
}
