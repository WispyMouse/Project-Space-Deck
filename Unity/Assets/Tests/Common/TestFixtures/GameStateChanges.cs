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
    using SpaceDeck.Utility.Logging;

    public class LoggingGameStateChange : GameStateChange
    {
        public static string LastLog = "";

        public readonly string ToLog;

        public LoggingGameStateChange(string toLog) : base(NobodyTarget.Instance)
        {
            this.ToLog = toLog;
        }

        public override void Apply(IGameStateMutator toApplyTo)
        {
            Logging.DebugLog(WellknownLoggingLevels.Debug, WellknownLoggingCategories.Test, this.ToLog);
            Debug.Log(this.ToLog);
            LastLog = this.ToLog;
        }

        public override string Describe()
        {
            throw new NotImplementedException();
        }
    }

    public class EvaluateAndLogNumericGameStateChange : GameStateChange
    {
        public readonly decimal Value;
        public readonly Action<decimal> EvaluateLogAction;

        public EvaluateAndLogNumericGameStateChange(decimal value, Action<decimal> evaluationAction) : base(NobodyTarget.Instance)
        {
            this.Value = value;
            this.EvaluateLogAction = evaluationAction;
        }

        public override void Apply(IGameStateMutator toApplyTo)
        {
            Debug.Log(this.Value.ToString());
            this.EvaluateLogAction.Invoke(this.Value);
        }

        public override string Describe()
        {
            throw new NotImplementedException();
        }
    }
}