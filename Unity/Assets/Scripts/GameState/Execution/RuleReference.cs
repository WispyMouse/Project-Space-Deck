namespace SpaceDeck.GameState.Execution
{
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Tokenization.Minimum.Context;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using SpaceDeck.Utility.Minimum;
    using SpaceDeck.Utility.Wellknown;

    public static class RuleReference
    {
        private static readonly List<Rule> Rules = new List<Rule>();
        private static readonly Dictionary<LowercaseString, HashSet<Rule>> EventsToRules = new Dictionary<LowercaseString, HashSet<Rule>>();
        private static readonly List<Rule> AlwaysConsideredRules = new List<Rule>();

        public static void RegisterRule(Rule toRegister)
        {
            Rules.Add(toRegister);

            HashSet<LowercaseString> applyOnEvents = toRegister.TriggerOnEventIds;
            if (applyOnEvents.Count == 0)
            {
                AlwaysConsideredRules.Add(toRegister);
            }
            else
            {
                foreach (LowercaseString eventId in applyOnEvents)
                {
                    if (!EventsToRules.TryGetValue(eventId, out HashSet<Rule> rules))
                    {
                        rules = new HashSet<Rule>();
                        EventsToRules.Add(eventId, rules);
                    }

                    rules.Add(toRegister);
                }
            }
        }

        public static void ClearRules()
        {
            Rules.Clear();
            EventsToRules.Clear();
            AlwaysConsideredRules.Clear();
        }

        public static List<GameStateChange> GetAppliedRules(GameStateDelta developingDelta, GameStateEventTrigger trigger)
        {
            List<GameStateChange> applications = new List<GameStateChange>();

            List<Rule> rulesToConsider = new List<Rule>();
            rulesToConsider.AddRange(AlwaysConsideredRules);

            if (EventsToRules.ContainsKey(trigger.EventId))
            {
                rulesToConsider.AddRange(EventsToRules[trigger.EventId]);
            }

            foreach (Rule curRule in rulesToConsider)
            {
                if (curRule.TryApplyRule(trigger, developingDelta, out List<GameStateChange> thisRuleApplications))
                {
                    applications.AddRange(thisRuleApplications);
                }
            }

            return applications;
        }
    }
}