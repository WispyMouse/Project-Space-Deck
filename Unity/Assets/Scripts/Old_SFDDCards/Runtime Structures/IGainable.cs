namespace SFDDCards
{
    using SFDDCards.ImportModels;
    using SFDDCards.ScriptingTokens.EvaluatableValues;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Models.Instances;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;

    /// <summary>
    /// Represents some amount of something to gain.
    /// Only one of <see cref="GainedEffect"/>, <see cref="GainedCard"/>, or <see cref="GainedCurrency"/> should return a value.
    /// The rest should be null.
    /// Callers can act as though only one has a value, in the above order.
    /// </summary>
    public interface IGainable
    {
        [Obsolete("Transition to " + nameof(_GainedEffect))]
        StatusEffect GainedEffect { get; }
        SpaceDeck.GameState.Minimum.AppliedStatusEffect _GainedEffect { get; }
        [Obsolete("Transition to " + nameof(_GainedCard))]
        Card GainedCard { get; }
        CardInstance _GainedCard { get; }
        Currency GainedCurrency { get; }

        IEvaluatableValue<int> GainedAmount { get; }
    }
}