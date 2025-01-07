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

    public class ExecuteOnTriggerRule : Rule
    {
        private Action<IGameStateMutator> ToExecute;

        public ExecuteOnTriggerRule(LowercaseString trigger, Action<IGameStateMutator> toExecute) : base(trigger)
        {
            this.ToExecute = toExecute;
        }

        public override bool TryApplyRule(GameStateEventTrigger trigger, IGameStateMutator gameStateMutator, out List<GameStateChange> applications)
        {
            Assert.IsTrue(this.TriggerOnEventIds.Contains(trigger.EventId), "Should only execute rule upon the correct rule event id triggering.");

            applications = new List<GameStateChange>() { new ActionExecutor(this.ToExecute) };
            return true;
        }
    }
}
