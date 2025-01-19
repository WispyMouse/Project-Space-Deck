namespace SpaceDeck.Utility.Unity
{
    using NUnit.Framework;
    using SpaceDeck.Utility.Minimum;
    using SpaceDeck.Utility.Wellknown;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class DebugLogger : MonoBehaviour
    {
        private static bool AssertFailureOnError = false;

        public static void SubscribeDebugListener(bool assertFailureOnError)
        {
            AssertFailureOnError = assertFailureOnError;
            Logging.Logging.DebugLoggingActionEvent += DebugLogging;
        }

        public static void UnsubscribeDebugListener()
        {
            Logging.Logging.DebugLoggingActionEvent -= DebugLogging;
        }

        public static void DebugLogging(LowercaseString logLevel, LowercaseString logCategory, string toLog)
        {
            if (logLevel == WellknownLoggingLevels.Error)
            {
                UnityEngine.Debug.LogError($"{logCategory}: {toLog}");

                if (AssertFailureOnError)
                {
                    Assert.Fail($"{logCategory}: {toLog}");
                }
            }
            else
            {
                UnityEngine.Debug.Log($"{logLevel} -> {logCategory}: {toLog}");
            }
        }
    }
}