namespace SFDDCards.ScriptingTokens
{
    using SFDDCards.Evaluation.Actual;
    using SFDDCards.Evaluation.Conceptual;
    using SFDDCards.ScriptingTokens.EvaluatableValues;
    using SpaceDeck.GameState.Execution;
    using System.Collections.Generic;

    public class ChooseCardScriptingToken : BaseScriptingToken, ILaterZoneListenerScriptingToken, IRealizedOperationScriptingToken
    {
        public IEvaluatableValue<int> NumberOfCards { get; private set; }
        public PromisedCardsEvaluatableValue PromisedCards { get; private set; } = new PromisedCardsEvaluatableValue();

        public string LaterRealizedDestinationZone
        {
            get
            {
                return _laterRealizedDestinationZone;
            }
            set
            {
                _laterRealizedDestinationZone = value;
                this.UpdateDescriptionForPromise();
            }
        }
        private string _laterRealizedDestinationZone { get; set; }

        public override string ScriptingTokenIdentifier { get; } = "CHOOSECARDS";

        public bool ShouldSilenceSpeaker => false;

        private ConceptualTokenEvaluatorBuilder FromBuilder;

        public override void ApplyToken(ConceptualTokenEvaluatorBuilder tokenBuilder)
        {
            throw new System.Exception("OBSOLETE");
        }

        protected override bool TryGetTokenWithArguments(List<string> arguments, out IScriptingToken scriptingToken)
        {
            scriptingToken = null;

            if (!TryGetIntegerEvaluatableFromStrings(arguments, out IEvaluatableValue<int> output, out List<string> _))
            {
                return false;
            }

            scriptingToken = new ChooseCardScriptingToken()
            {
                NumberOfCards = output
            };

            return true;
        }

        void UpdateDescriptionForPromise()
        {
            string numberEval;

            if (this.NumberOfCards is ConstantNumericEvaluatableValue constant && constant.ConstantValue == 1)
            {
                numberEval = "a";
            }
            else
            {
                numberEval = this.NumberOfCards.DescribeEvaluation();
            }

            if (!string.IsNullOrEmpty(LaterRealizedDestinationZone) && LaterRealizedDestinationZone == "discard" && this.PromisedCards.SampledPool != null && this.PromisedCards.SampledPool is HandCardsEvaluatableValue)
            {
                this.PromisedCards.DescriptionText = $"{numberEval} {EffectDescriberDatabase.ExtractSingularOrPlural(this.NumberOfCards, "card")}";
            }
            else if (this.PromisedCards.SampledPool != null)
            {
                this.PromisedCards.DescriptionText = $"{numberEval} {EffectDescriberDatabase.ExtractSingularOrPlural(this.NumberOfCards, "card")} from {this.PromisedCards.SampledPool.DescribeEvaluation()}";
            }
        }

        public string DescribeOperationAsEffect(ConceptualDeltaEntry delta, string reactionWindowId)
        {
            return string.Empty;
        }

        public string DescribeOperationAsEffect(TokenEvaluatorBuilder builder)
        {
            return string.Empty;
        }

        public void ApplyToDelta(DeltaEntry applyingDuringEntry, ReactionWindowContext? context, out List<DeltaEntry> stackedDeltas)
        {
            stackedDeltas = null;
        }
    }
}