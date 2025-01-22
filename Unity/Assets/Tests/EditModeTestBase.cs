namespace SpaceDeck.Tests.EditMode.Execution
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
    using System.Linq;
    using SpaceDeck.Utility.Minimum;
    using SpaceDeck.Models.Databases;
    using SpaceDeck.Models.Imports;
    using SpaceDeck.Utility.Wellknown;
    using SpaceDeck.GameState.Deltas;
    using SpaceDeck.Utility.Unity;
    using UnityEngine.TestTools;

    public abstract class EditModeTestBase
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            DebugLogger.SubscribeDebugListener(true);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            DebugLogger.UnsubscribeDebugListener();
        }

        [SetUp]
        public void SetUp()
        {
            DebugLogger.AssertFailureOnError = true;
            LogAssert.ignoreFailingMessages = false;
        }

        [TearDown]
        public void TearDown()
        {
            CommonTestUtility.TearDownDatabases();
            LogAssert.ignoreFailingMessages = false;
        }
    }
}