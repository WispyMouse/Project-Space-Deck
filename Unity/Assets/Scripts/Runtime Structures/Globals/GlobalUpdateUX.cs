using SFDDCards;
using SFDDCards.Evaluation.Actual;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SFDDCards
{
    public static class GlobalUpdateUX
    {
        public enum LogType
        {
            Info,
            GameEvent,
            RuntimeError,
            UserError
        }

        public static UnityEvent<CampaignContext> UpdateUXEvent = new UnityEvent<CampaignContext>();
        public static UnityEvent<string, LogType> LogTextEvent = new UnityEvent<string, LogType>();

        public static UnityEvent<DeltaEntry, PlayerChoice, Action> PlayerMustMakeChoice = new UnityEvent<DeltaEntry, PlayerChoice, Action>();
        public static bool PendingPlayerChoice = false;

        static GlobalUpdateUX()
        {
            LogTextEvent.AddListener(LogText);
        }

        private static void LogText(string toLog, LogType type)
        {
            if (string.IsNullOrEmpty(toLog))
            {
                return;
            }

            toLog = toLog.Trim();

            switch (type)
            {
                case LogType.Info:
                case LogType.GameEvent:
                case LogType.UserError:
                    Debug.Log(toLog);
                    break;
                case LogType.RuntimeError:
                    Debug.LogError(toLog);
                    break;
            }
        }
    }
}