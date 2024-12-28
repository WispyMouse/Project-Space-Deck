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

    public class ExecuteScriptingCommand : ScriptingCommand
    {
        public static readonly LowercaseString IdentifierString = new LowercaseString("ExecuteScriptingCommand");
        public override LowercaseString Identifier => IdentifierString;

        public override bool TryGetLinkedToken(ParsedToken parsedToken, out LinkedToken linkedToken)
        {
            throw new System.NotImplementedException("You should not parse this token. Instead, directly create an ExecuteLinkedToken.");
        }
    }

    public class ExecuteLinkedToken : LinkedToken<ExecuteScriptingCommand>
    {
        public readonly Action<IGameStateMutator> Action;

        public ExecuteLinkedToken(Action<IGameStateMutator> action) : base()
        {
            this.Action = action;
        }

        public override bool TryGetChanges(ScriptingExecutionContext context, out List<GameStateChange> changes)
        {
            changes = new List<GameStateChange>();
            changes.Add(new ActionExecutor(this.Action));
            return true;
        }
    }

    /// <summary>
    /// Based on <see cref="ExecuteWithTargetLinkedToken"/>, but requires a target in its questions.
    /// </summary>
    public class ExecuteWithTargetLinkedToken : ExecuteLinkedToken
    {
        public readonly ChangeTargetEvaluatableValue ChangeTarget;

        public ExecuteWithTargetLinkedToken(ChangeTargetEvaluatableValue changeTarget, Action<IGameStateMutator> action) : base(action)
        {
            this.ChangeTarget = changeTarget;
            this._Questions.AddRange(changeTarget.GetQuestions(this));
        }

        public override bool TryGetChanges(ScriptingExecutionContext context, out List<GameStateChange> changes)
        {
            if (!this.ChangeTarget.TryEvaluate(context, out IChangeTarget target))
            {
                changes = null;
                return false;
            }

            changes = new List<GameStateChange>();
            changes.Add(new ActionExecutor(this.Action));
            return true;
        }
    }

    public class ZeroArgumentDebugLogScriptingCommand : ScriptingCommand
    {
        public const string HelloString = "HELLO!";
        public static readonly LowercaseString IdentifierString = new LowercaseString("ZEROARGUMENTDEBUG");
        public override LowercaseString Identifier => IdentifierString;

        public override bool TryGetLinkedToken(ParsedToken parsedToken, out LinkedToken linkedToken)
        {
            linkedToken = new ZeroArgumentDebugLogLinkedToken(parsedToken);
            return true;
        }
    }

    public class ZeroArgumentDebugLogLinkedToken : LinkedToken<ZeroArgumentDebugLogScriptingCommand>
    {
        public ZeroArgumentDebugLogLinkedToken(ParsedToken parsedToken) : base(parsedToken)
        {
        }

        public override bool TryGetChanges(ScriptingExecutionContext context, out List<GameStateChange> changes)
        {
            changes = new List<GameStateChange>();
            changes.Add(new LoggingGameStateChange(ZeroArgumentDebugLogScriptingCommand.HelloString));
            return true;
        }
    }

    public class OneArgumentDebugLogScriptingCommand : ScriptingCommand
    {
        public static readonly LowercaseString IdentifierString = new LowercaseString("ONEARGUMENTDEBUG");
        public override LowercaseString Identifier => IdentifierString;

        public override bool TryGetLinkedToken(ParsedToken parsedToken, out LinkedToken linkedToken)
        {
            linkedToken = new OneArgumentDebugLogLinkedToken(parsedToken);
            return true;
        }
    }

    public class OneArgumentDebugLogLinkedToken : LinkedToken<OneArgumentDebugLogScriptingCommand>
    {
        public OneArgumentDebugLogLinkedToken(ParsedToken parsedToken) : base(parsedToken)
        {
        }

        public override bool TryGetChanges(ScriptingExecutionContext context, out List<GameStateChange> changes)
        {
            if (this.Arguments == null || this.Arguments.Count == 0)
            {
                changes = null;
                return false;
            }

            changes = new List<GameStateChange>();
            changes.Add(new LoggingGameStateChange(this.Arguments[0].ToString()));
            return true;
        }
    }

    public class HelloWorldScriptingCommand : ScriptingCommand
    {
        public static readonly LowercaseString IdentifierString = new LowercaseString("HELLOWORLD");
        public override LowercaseString Identifier => IdentifierString;
    }

    public class TwoArgumentScriptingCommand : ScriptingCommand
    {
        public static readonly LowercaseString IdentifierString = new LowercaseString("TWOARGUMENTS");
        public override LowercaseString Identifier => IdentifierString;
    }
}