namespace SFDDCards
{
    using SFDDCards.Evaluation.Actual;
    using SFDDCards.Evaluation.Conceptual;
    using SFDDCards.ScriptingTokens;
    using SFDDCards.ScriptingTokens.EvaluatableValues;
    using SpaceDeck.GameState.Minimum;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using UnityEngine;

    public abstract class PlayerChoice
    {
        public bool ResultIsChosen { get; protected set; } = false;
        [Obsolete("Transition to " + nameof(_DescribeChoice))]
        public abstract string DescribeChoice(CampaignContext campaignContext, TokenEvaluatorBuilder currentEvaluator);
        public abstract string _DescribeChoice(IGameStateMutator mutator);
        public abstract bool TryFinalizeWithoutPlayerInput(DeltaEntry toApplyTo);
    }

    public abstract class PlayerChoice<T> : PlayerChoice
    {
        public T ChosenResult { get; private set; }
        public virtual void SetChoice(DeltaEntry toApplyTo, T result)
        {
            this.ChosenResult = result;
            this.ResultIsChosen = true;
        }
    }
}