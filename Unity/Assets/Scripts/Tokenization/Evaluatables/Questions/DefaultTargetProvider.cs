namespace SpaceDeck.Tokenization.Evaluatables.Questions
{
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Tokenization.Evaluatables;
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.Tokenization.Minimum.Context;
    using SpaceDeck.Tokenization.Minimum.Questions;
    using System;
    using System.Collections.Generic;

    public class DefaultTargetProvider : ChangeTargetProvider
    {
        public static readonly DefaultTargetProvider Instance = new DefaultTargetProvider();

        private DefaultTargetProvider()
        {

        }

        public override IReadOnlyList<IChangeTarget> GetProvidedTargets(QuestionAnsweringContext answeringContext)
        {
            if (answeringContext.DefaultTarget == null)
            {
                return null;
            }
            else
            {
                return new List<IChangeTarget>() { answeringContext.DefaultTarget };
            }
        }

        public override IReadOnlyList<IChangeTarget> GetProvidedTargets(IGameStateMutator mutator)
        {
            return null;
        }

        public override string Describe()
        {
            return string.Empty;
        }
    }
}