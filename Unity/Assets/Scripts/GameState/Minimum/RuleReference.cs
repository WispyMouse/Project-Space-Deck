namespace SpaceDeck.GameState.Minimum
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using SpaceDeck.Utility.Minimum;
    using SpaceDeck.Utility.Wellknown;
    using static SpaceDeck.GameState.Minimum.GameStateEventTrigger;

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

        public static List<GameStateChange> GetAppliedRules(IGameStateMutator mutator, TriggerDirection direction, GameStateEventTrigger trigger)
        {
            List<GameStateChange> applications = new List<GameStateChange>();

            List<Rule> rulesToConsider = new List<Rule>();
            rulesToConsider.AddRange(AlwaysConsideredRules);

            if (EventsToRules.ContainsKey(trigger.EventId))
            {
                rulesToConsider.AddRange(EventsToRules[trigger.EventId]);
            }

            // Sort it such that "higher priority" is later in the list
            rulesToConsider.Sort((Rule a, Rule b) => { return a.RulePriorityOrder.CompareTo(b.RulePriorityOrder); });

            foreach (Rule curRule in rulesToConsider)
            {
                if (curRule.TryApplyRule(trigger, direction, mutator, out List<GameStateChange> thisRuleApplications))
                {
                    applications.AddRange(thisRuleApplications);
                }
            }

            return applications;
        }
    }
}