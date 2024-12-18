namespace SpaceDeck.Tokenization.Evaluatables.Questions
{
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Tokenization.Evaluatables;
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.Tokenization.Minimum.Context;
    using SpaceDeck.Tokenization.Minimum.Questions;
    using System;
    using System.Collections.Generic;

    public class SpecificTargetProvider : ChangeTargetProvider
    {
        public readonly IReadOnlyList<IChangeTarget> Options = Array.Empty<IChangeTarget>();

        public SpecificTargetProvider(IChangeTarget target)
        {
            this.Options = new List<IChangeTarget>() { target };
        }

        public SpecificTargetProvider(IReadOnlyList<IChangeTarget> target)
        {
            this.Options = target;
        }

        public override IReadOnlyList<IChangeTarget> GetProvidedTargets(QuestionAnsweringContext answeringContext)
        {
            return this.Options;
        }
    }
}