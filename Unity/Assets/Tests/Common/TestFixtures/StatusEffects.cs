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
    using SpaceDeck.GameState.Changes.Targets;
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

        public override bool TryApplyStatusEffect(GameStateEventTrigger trigger, IGameStateMutator gameStateMutator, out List<GameStateChange> applications)
        {
            applications = new List<GameStateChange>()
            {
                new ActionExecutor(this.ToExecute)
            };

            return true;
        }
    }

    public class DebugPoisonStatusEffectPrototype : StatusEffectPrototype
    {
        public DebugPoisonStatusEffectPrototype() : base(nameof(DebugPoisonStatusEffectPrototype), nameof(DebugPoisonStatusEffectPrototype))
        {
        }
    }

    public class DebugPoisonStatusEffectInstance : AppliedStatusEffect
    {
        public DebugPoisonStatusEffectInstance() : base(nameof(DebugPoisonStatusEffectPrototype), nameof(DebugPoisonStatusEffectPrototype), new HashSet<LowercaseString>() { WellknownGameStateEvents.EntityTurnStarted })
        {
        }

        public override bool TryApplyStatusEffect(GameStateEventTrigger trigger, IGameStateMutator gameStateMutator, out List<GameStateChange> applications)
        {
            if (trigger.BasedOnTarget == null)
            {
                applications = null;
                return false;
            }

            applications = new List<GameStateChange>();
            foreach (Entity representedEntity in trigger.BasedOnTarget.GetRepresentedEntities(gameStateMutator))
            {
                // How many stacks?
                int numberOfStacks = gameStateMutator.GetStacks(representedEntity, this.Id);

                // Only apply if there are a positive number of stacks
                if (numberOfStacks <= 0)
                {
                    continue;
                }

                // Damage equal to stack amount
                applications.Add(new ModifyNumericQuality(representedEntity, representedEntity, WellknownQualities.Health, -gameStateMutator.GetStacks(representedEntity, this.Id)));

                // Remove one stack
                applications.Add(new ModifyStatusEffectStacks(representedEntity, this.Id, -1));
            }

            return true;
        }
    }
}