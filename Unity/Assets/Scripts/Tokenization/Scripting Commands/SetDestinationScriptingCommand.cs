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

    public class SetDestinationScriptingCommand : ScriptingCommand
    {
        public override LowercaseString Identifier => "DESTINATION";

        public override bool TryGetLinkedToken(ParsedToken parsedToken, out LinkedToken linkedToken)
        {
            if (parsedToken.Arguments.Count != 1)
            {
                linkedToken = null;
                return false;
            }

            linkedToken = new SetDestinationLinkedToken(new CardsEvaluatableValue(PlayedCardProvider.Instance), parsedToken, parsedToken.Arguments[0]);
            return true;
        }
    }

    public class SetDestinationLinkedToken : LinkedToken<SetDestinationScriptingCommand>
    {
        public readonly CardsEvaluatableValue CardEvalautable;
        public readonly LowercaseString Destination;

        public SetDestinationLinkedToken(CardsEvaluatableValue card, ParsedToken parsedToken, LowercaseString destination) : base(parsedToken)
        {
            this.CardEvalautable = card;
            this.Destination = destination;
        }

        public override bool TryGetChanges(ScriptingExecutionContext context, out List<GameStateChange> changes)
        {
            if (!this.CardEvalautable.TryEvaluate(context.ExecutedOnGameState, out IReadOnlyList<CardInstance> cards))
            {
                changes = null;
                return false;
            }

            changes = new List<GameStateChange>();

            foreach (CardInstance card in cards)
            {
                changes.Add(new SetStringQuality(null, card, WellknownQualities.Destination, this.Destination));
            }

            return true;
        }
    }
}
