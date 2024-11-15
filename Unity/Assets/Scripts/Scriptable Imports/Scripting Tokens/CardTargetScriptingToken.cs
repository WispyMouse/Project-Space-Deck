namespace SFDDCards.ScriptingTokens
{
    using SFDDCards.Evaluation.Actual;
    using SFDDCards.Evaluation.Conceptual;
    using SFDDCards.ScriptingTokens.EvaluatableValues;
    using System.Collections.Generic;

    public class CardTargetScriptingToken : BaseScriptingToken
    {
        public override string ScriptingTokenIdentifier { get; } = "CARDTARGET";
        public string Zone { get; set; }
        public IEvaluatableValue<int> NumberOfCards { get; set; } = null;
        public CardsEvaluatableValue Cards { get; set; } = null;

        public override void ApplyToken(ConceptualTokenEvaluatorBuilder tokenBuilder)
        {
            if (string.IsNullOrEmpty(Zone) || Zone.ToLower() == "self")
            {
                if (!(tokenBuilder.Owner is Card ownedCard))
                {
                    GlobalUpdateUX.LogTextEvent.Invoke($"Told to target this card, but the owner of the effect isn't a card.", GlobalUpdateUX.LogType.RuntimeError);
                    return;
                }

                this.Cards = new SelfCardEvaluatableValue(ownedCard);
                tokenBuilder.RelevantCards = this.Cards;
            }
            else
            {
                this.Cards = CardsEvaluatableValue.GetEvaluatable(this.Zone, this.NumberOfCards);
                tokenBuilder.RelevantCards = this.Cards;
            }
        }

        protected override bool TryGetTokenWithArguments(List<string> arguments, out IScriptingToken scriptingToken)
        {
            scriptingToken = null;

            TryGetIntegerEvaluatableFromStrings(arguments, out IEvaluatableValue<int> output, out List<string> remainingArguments);

            if (remainingArguments.Count != 0 && remainingArguments.Count != 1)
            {
                return false;
            }

            if (remainingArguments.Count == 0)
            {
                scriptingToken = new CardTargetScriptingToken()
                {
                    NumberOfCards = null,
                    Zone = "self"
                };
                return true;
            }

            scriptingToken = new CardTargetScriptingToken()
            {
                NumberOfCards = output,
                Zone = arguments[0].ToLower()
            };

            return true;
        }
    }
}