namespace SpaceDeck.UX
{
    using UnityEngine;
    using SpaceDeck.Utility.Unity;

    public class LoggingSubscriber : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void SubscribeUnityLogger()
        {
            DebugLogger.SubscribeDebugListener(false);
        }
    }
}