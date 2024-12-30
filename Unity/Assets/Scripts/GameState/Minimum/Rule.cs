namespace SpaceDeck.GameState.Minimum
{
    using SpaceDeck.GameState.Minimum;
    using System.Collections;
    using System.Collections.Generic;
    using SpaceDeck.Utility.Minimum;

    public abstract class Rule
    {
        /// <summary>
        /// When an event with one of these ids happens, the rule will try to apply.
        /// If this list is empty, then the rule applies *constantly*, whenever *anything* happens.
        /// </summary>
        public readonly HashSet<LowercaseString> TriggerOnEventIds = new HashSet<LowercaseString>();
        public readonly int RulePriorityOrder = 0;

        public Rule(int priorityOrder)
        {
            this.RulePriorityOrder = priorityOrder;
        }

        public Rule(HashSet<LowercaseString> triggerOnEventIds, int priorityOrder = 0) : this(priorityOrder)
        {
            this.TriggerOnEventIds = triggerOnEventIds;
        }

        public Rule(LowercaseString triggerOneventId, int priorityOrder = 0) : this(new HashSet<LowercaseString>() { triggerOneventId }, priorityOrder)
        {

        }

        public virtual bool TryApplyRule(GameStateEventTrigger trigger, IGameStateMutator gameStateMutator, out List<GameStateChange> applications)
        {
            applications = null;
            return false;
        }
    }
}