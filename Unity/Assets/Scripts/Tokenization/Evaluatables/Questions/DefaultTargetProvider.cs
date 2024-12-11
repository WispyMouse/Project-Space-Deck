namespace SpaceDeck.Tokenization.Evaluatables.Questions
{
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Tokenization.Evaluatables;
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.Tokenization.Minimum.Questions;
    using System.Collections.Generic;

    public class DefaultTargetProvider : ChangeTargetProvider
    {
        public static readonly DefaultTargetProvider Instance = new DefaultTargetProvider();

        private DefaultTargetProvider()
        {

        }

        public override IChangeTarget ChooseByIndex(int index)
        {
            throw new System.NotImplementedException();
        }
    }
}