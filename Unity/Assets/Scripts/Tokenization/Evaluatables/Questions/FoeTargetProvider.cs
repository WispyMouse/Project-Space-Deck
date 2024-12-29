namespace SpaceDeck.Tokenization.Evaluatables
{
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.GameState.Minimum;
    using System.Collections.Generic;
    using SpaceDeck.Tokenization.Minimum.Questions;
    using SpaceDeck.Tokenization.Minimum.Context;
    using SpaceDeck.Tokenization.Evaluatables.Questions;
    using SpaceDeck.Utility.Minimum;
    using SpaceDeck.Utility.Wellknown;

    public class FoeTargetProvider : ChangeTargetProvider
    {
        public static readonly FoeTargetProvider Instance = new FoeTargetProvider();

        private FoeTargetProvider()
        {

        }

        public override IReadOnlyList<IChangeTarget> GetProvidedTargets(QuestionAnsweringContext answeringContext)
        {
            List<IChangeTarget> foes = new List<IChangeTarget>();
            foreach (IChangeTarget target in answeringContext.StartingGameState.GetAllEntities())
            {
                bool includeTarget = true;

                foreach (Entity representedEntity in target.GetRepresentedEntities(answeringContext.StartingGameState))
                {
                    if (answeringContext.StartingGameState.GetNumericQuality(representedEntity, WellknownQualities.Faction) != WellknownFactions.Foe)
                    {
                        includeTarget = false;
                        break;
                    }
                }

                if (includeTarget)
                {
                    foes.Add(target);
                }
            }
            return foes;
        }

        public override IReadOnlyList<IChangeTarget> GetProvidedTargets(IGameStateMutator mutator)
        {
            List<IChangeTarget> foes = new List<IChangeTarget>();
            foreach (IChangeTarget target in mutator.GetAllEntities())
            {
                bool includeTarget = true;

                foreach (Entity representedEntity in target.GetRepresentedEntities(mutator))
                {
                    if (mutator.GetNumericQuality(representedEntity, WellknownQualities.Faction) != WellknownFactions.Foe)
                    {
                        includeTarget = false;
                        break;
                    }
                }

                if (includeTarget)
                {
                    foes.Add(target);
                }
            }
            return foes;
        }

        public override string Describe()
        {
            return "Foe";
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