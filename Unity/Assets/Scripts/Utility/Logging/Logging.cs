namespace SpaceDeck.Utility.Logging
{
    using SpaceDeck.Utility.Minimum;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public delegate void LoggingAction(LowercaseString logLevel, LowercaseString logCategory, string log);

    public static class Logging
    {
        public static event LoggingAction DebugLoggingActionEvent;

        public static void DebugLog(LowercaseString logLevel, LowercaseString logCategory, string log)
        {
            DebugLoggingActionEvent?.Invoke(logLevel, logCategory, log);
        }
    }
}