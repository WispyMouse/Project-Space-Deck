namespace SFDDCards.ScriptingTokens
{
    using SFDDCards.Evaluation.Actual;
    using SFDDCards.Evaluation.Conceptual;
    using SFDDCards.ScriptingTokens.EvaluatableValues;
    using System.Collections.Generic;

    public class MoveCardToZoneScriptingToken : BaseScriptingToken, IRealizedOperationScriptingToken
    {
        public string Zone { get; private set; }

        public override string ScriptingTokenIdentifier { get; } = "MOVECARDTOZONE";

        public override void ApplyToken(ConceptualTokenEvaluatorBuilder tokenBuilder)
        {
            if (tokenBuilder.RelevantCards == null)
            {
                GlobalUpdateUX.LogTextEvent.Invoke("Attempted to apply an action that requires relevant cards selected without any.", GlobalUpdateUX.LogType.RuntimeError);
            }

            tokenBuilder.RealizedOperationScriptingToken = this;
        }

        protected override bool TryGetTokenWithArguments(List<string> arguments, out IScriptingToken scriptingToken)
        {
            scriptingToken = null;

            if (arguments.Count != 1)
            {
                return false;
            }

            scriptingToken = new MoveCardToZoneScriptingToken()
            {
                Zone = arguments[0].ToLower()
            };

            return true;
        }

        public override bool IsHarmfulToTarget(ICombatantTarget user, ICombatantTarget target)
        {
            return false;
        }

        public override bool RequiresTarget()
        {
            return false;
        }

        public string DescribeOperationAsEffect(ConceptualDeltaEntry delta, string reactionWindowId)
        {
            if (Zone == CardsEvaluatableValue.DiscardZoneId)
            {
                return $"Discard {delta.MadeFromBuilder.RelevantCards.DescribeEvaluation()}";
            }
            else if (Zone == CardsEvaluatableValue.ExileZoneId)
            {
                return $"Exile {delta.MadeFromBuilder.RelevantCards.DescribeEvaluation()}";
            }
            else if (Zone == CardsEvaluatableValue.DeckZoneId)
            {
                return $"Put {delta.MadeFromBuilder.RelevantCards.DescribeEvaluation()} into the deck and shuffle";
            }
            else if (Zone == CardsEvaluatableValue.HandZoneId)
            {
                return $"Put {delta.MadeFromBuilder.RelevantCards.DescribeEvaluation()} in hand";
            }

            return $"Move {delta.MadeFromBuilder.RelevantCards.DescribeEvaluation()} to {Zone}";
        }

        public void ApplyToDelta(DeltaEntry applyingDuringEntry, ReactionWindowContext? context, out List<DeltaEntry> stackedDeltas)
        {
            stackedDeltas = null;

            if (applyingDuringEntry.RelevantCards == null)
            {
                GlobalUpdateUX.LogTextEvent.Invoke($"Relevant cards list is null. Cannot move cards.", GlobalUpdateUX.LogType.RuntimeError);
                return;
            }

            if (!applyingDuringEntry.RelevantCards.TryEvaluateValue(applyingDuringEntry.FromCampaign, applyingDuringEntry.MadeFromBuilder, out List<Card> cards))
            {
                GlobalUpdateUX.LogTextEvent.Invoke($"Failed to parse relevant cards.", GlobalUpdateUX.LogType.RuntimeError);
                return;
            }

            List<Card> targetZone = null;

            if (this.Zone == CardsEvaluatableValue.DiscardZoneId)
            {
                targetZone = applyingDuringEntry.FromCampaign.CurrentCombatContext.PlayerCombatDeck.CardsCurrentlyInDiscard;
            }
            else if (this.Zone == CardsEvaluatableValue.HandZoneId)
            {
                targetZone = applyingDuringEntry.FromCampaign.CurrentCombatContext.PlayerCombatDeck.CardsCurrentlyInHand;
            }
            else if (this.Zone == CardsEvaluatableValue.ExileZoneId)
            {
                targetZone = applyingDuringEntry.FromCampaign.CurrentCombatContext.PlayerCombatDeck.CardsCurrentlyInExile;
            }
            else if (this.Zone == CardsEvaluatableValue.DeckZoneId)
            {
                targetZone = applyingDuringEntry.FromCampaign.CurrentCombatContext.PlayerCombatDeck.CardsCurrentlyInDeck;
            }

            if (targetZone == null)
            {
                GlobalUpdateUX.LogTextEvent.Invoke($"Could not determine how to move cards to zone '{this.Zone}'.", GlobalUpdateUX.LogType.RuntimeError);
            }

            foreach (Card curCard in new List<Card>(cards))
            {
                applyingDuringEntry.FromCampaign.CurrentCombatContext.PlayerCombatDeck.MoveCardToZone(curCard, targetZone);
            }
        }
    }
}