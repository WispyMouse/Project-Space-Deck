namespace SpaceDeck.GameState.Execution
{
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Tokenization.Minimum.Context;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public static class RuleReference
    {
        private static readonly List<Rule> Rules = new List<Rule>();

        public static void RegisterRule(Rule toRegister)
        {
            Rules.Add(toRegister);
        }

        public static void ClearRules()
        {
            Rules.Clear();
        }

        public static List<GameStateChange> GetAppliedRules(ScriptingExecutionContext context, GameStateChange change)
        {
            List<GameStateChange> applications = new List<GameStateChange>();

            foreach (Rule curRule in Rules)
            {
                if (curRule.TryApplyRule(context, change, out List<GameStateChange> thisRuleApplications))
                {
                    applications.AddRange(thisRuleApplications);
                }
            }

            return applications;
        }
    }
}