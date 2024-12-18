namespace SpaceDeck.Tokenization.Evaluatables
{
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.GameState.Minimum;
    using System.Collections.Generic;
    using SpaceDeck.Tokenization.Minimum.Questions;
    using SpaceDeck.Tokenization.Minimum.Context;
    using SpaceDeck.Tokenization.Evaluatables.Questions;

    public class FoeTargetProvider : ChangeTargetProvider
    {
        public static readonly FoeTargetProvider Instance = new FoeTargetProvider();

        private FoeTargetProvider()
        {

        }

        public override IReadOnlyList<IChangeTarget> GetProvidedTargets(QuestionAnsweringContext answeringContext)
        {
            // TODO: Only return foes of the user! This currently just gets everything.
            return answeringContext.StartingGameState.PersistentEntities;
        }
    }

    public class FoeTargetEvaluatableParser : EvaluatableParser<ChangeTargetEvaluatableValue, IChangeTarget>
    {
        private static readonly LowercaseString FoeText = new LowercaseString("FOE");

        public override bool TryParse(LowercaseString argument, out IEvaluatableValue<IChangeTarget> parsedValue)
        {
            if (argument.Equals(FoeText))
            {
                parsedValue = new ChangeTargetEvaluatableValue(FoeTargetProvider.Instance);
                return true;
            }

            parsedValue = null;
            return false;
        }
    }
}