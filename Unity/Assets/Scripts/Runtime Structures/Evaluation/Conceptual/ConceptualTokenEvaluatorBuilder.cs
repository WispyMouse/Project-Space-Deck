namespace SFDDCards.Evaluation.Conceptual
{
    using SFDDCards.Evaluation.Actual;
    using SFDDCards.ScriptingTokens;
    using SFDDCards.ScriptingTokens.EvaluatableValues;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using UnityEngine;
    using static SFDDCards.Evaluation.Actual.TokenEvaluatorBuilder;

    public class ConceptualTokenEvaluatorBuilder
    {
        public List<ElementResourceChange> ElementResourceChanges = new List<ElementResourceChange>();
        public List<IScriptingToken> AppliedTokens = new List<IScriptingToken>();

        public Dictionary<Element, IEvaluatableValue<int>> ElementRequirements = new Dictionary<Element, IEvaluatableValue<int>>();
        public List<RequiresComparisonScriptingToken> RequiresComparisons = new List<RequiresComparisonScriptingToken>();

        public CombatantTargetEvaluatableValue Target;
        public CombatantTargetEvaluatableValue OriginalTarget;

        public IEvaluatableValue<int> Intensity;
        public IntensityKind IntensityKindType;
        public NumberOfCardsRelation NumberOfCardsRelationType = NumberOfCardsRelation.None;

        public StatusEffect StatusEffect;

        public List<Action<DeltaEntry>> ActionsToExecute = new List<Action<DeltaEntry>>();

        public ConceptualTokenEvaluatorBuilder(ConceptualTokenEvaluatorBuilder previousBuilder = null)
        {
            if (previousBuilder != null)
            {
                this.ElementRequirements = new Dictionary<Element, IEvaluatableValue<int>>(previousBuilder.ElementRequirements);
                this.OriginalTarget = previousBuilder.OriginalTarget;
                this.Target = previousBuilder.Target;
            }
        }

        public bool HasSameElementRequirement(ConceptualTokenEvaluatorBuilder previous)
        {
            if (previous == null)
            {
                return false;
            }

            if (this.ElementRequirements.Count != previous.ElementRequirements.Count)
            {
                return false;
            }

            foreach (Element elementKey in this.ElementRequirements.Keys)
            {
                if (!previous.ElementRequirements.TryGetValue(elementKey, out IEvaluatableValue<int> value))
                {
                    return false;
                }

                if (value != this.ElementRequirements[elementKey])
                {
                    return false;
                }
            }

            return true;
        }

        public string DescribeElementRequirements()
        {
            if (this.ElementRequirements.Count == 0)
            {
                return string.Empty;
            }

            StringBuilder compositeRequirements = new StringBuilder();
            compositeRequirements.Append("Requires: ");
            string startingComma = "";
            bool nonzeroFound = false;

            foreach (Element element in this.ElementRequirements.Keys)
            {
                compositeRequirements.Append($"{startingComma}{this.ElementRequirements[element].DescribeEvaluation()} {element.GetNameOrIcon()}");
                startingComma = ", ";
                nonzeroFound = true;
            }

            if (!nonzeroFound)
            {
                return string.Empty;
            }

            return compositeRequirements.ToString();
        }

        public ConceptualDelta GetConceptualDelta()
        {
            ConceptualDelta delta = new ConceptualDelta();

            delta.DeltaEntries.Add(new ConceptualDeltaEntry(this, this.OriginalTarget, this.Target)
            {
                MadeFromBuilder = this,
                ConceptualTarget = this.Target,
                ConceptualIntensity = this.Intensity,
                IntensityKindType = this.IntensityKindType,
                NumberOfCardsRelationType = this.NumberOfCardsRelationType,
                ElementResourceChanges = this.ElementResourceChanges,
                StatusEffect = this.StatusEffect
            });

            return delta;
        }
    }
}