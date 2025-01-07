namespace SpaceDeck.Tests.EditMode.Common.TestFixtures
{
    using NUnit.Framework;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using UnityEngine;
    using UnityEngine.Events;
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.Tokenization.Processing;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.GameState.Execution;
    using SpaceDeck.Tokenization.ScriptingCommands;
    using SpaceDeck.GameState.Changes;
    using SpaceDeck.Tokenization.Evaluatables;
    using SpaceDeck.Tokenization.Minimum.Questions;
    using SpaceDeck.GameState.Context;
    using SpaceDeck.Tokenization.Evaluatables.Questions;
    using SpaceDeck.Tokenization.Minimum.Context;
    using SpaceDeck.Utility.Minimum;
    using SpaceDeck.Utility.Wellknown;
    using SpaceDeck.Models.Prototypes;
    using static SpaceDeck.GameState.Minimum.GameStateEventTrigger;

    public class ExecuteAppliedStatusEffectPrototype : StatusEffectPrototype
    {
        public ExecuteAppliedStatusEffectPrototype(LowercaseString id, string name) : base(id, name)
        {
        }
    }

    public class ExecuteAppliedStatusEffectInstance : AppliedStatusEffect
    {
        public readonly Action<IGameStateMutator> ToExecute;

        public ExecuteAppliedStatusEffectInstance(Action<IGameStateMutator> toExecute, IEnumerable<LowercaseString> triggerOnEventIds, string name, LowercaseString id) : base(id, name, triggerOnEventIds)
        {
            this.ToExecute = toExecute;
        }

        public override bool TryApplyStatusEffect(GameStateEventTrigger trigger, IGameStateMutator gameStateMutator, TriggerDirection direction, out List<GameStateChange> applications)
        {
            applications = new List<GameStateChange>()
            {
                new ActionExecutor(this.ToExecute)
            };

            return true;
        }
    }
}