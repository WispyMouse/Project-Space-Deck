namespace SpaceDeck.GameState.Execution
{
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Models.Databases;
    using SpaceDeck.Utility.Minimum;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a list of <see cref="GameStateChange"/>.
    /// This can be used to represent what will happen when
    /// an entire effect resolves.
    /// </summary>
    public class GameStateDelta : IGameStateMutator
    {
        public readonly List<GameStateChange> Changes = new List<GameStateChange>();

        public enum EntityDestination
        {
            Removed,
            Encounter,
            Persistent
        }

        public EntityTurnTakerCalculator EntityTurnTakerCalculator { get => BaseGameState.EntityTurnTakerCalculator; set => BaseGameState.EntityTurnTakerCalculator = value; }
        public FactionTurnTakerCalculator FactionTurnTakerCalculator { get => BaseGameState.FactionTurnTakerCalculator; set => BaseGameState.FactionTurnTakerCalculator = value; }

        public readonly IGameStateMutator BaseGameState;
        public readonly PendingResolveStack PendingResolves = new PendingResolveStack();

        public readonly Dictionary<IHaveQualities, QualitiesHolder> DeltaValues = new Dictionary<IHaveQualities, QualitiesHolder>();

        public readonly Dictionary<Entity, EntityDestination> EntityDestinationChanges = new Dictionary<Entity, EntityDestination>();
        public readonly Dictionary<CardInstance, LowercaseString> CardDestinationChanges = new Dictionary<CardInstance, LowercaseString>();

        public decimal? NewFactionTurn = null;
        public Entity NewEntityTurn = null;

        public readonly Dictionary<Currency, int> CurrencyChanges = new Dictionary<Currency, int>();

        public GameStateDelta(IGameStateMutator baseGameState)
        {
            this.BaseGameState = baseGameState;
        }

        public GameStateDelta(IGameStateMutator baseGameState, IEnumerable<GameStateChange> changes) : this (baseGameState)
        {
            foreach (GameStateChange change in changes)
            {
                change.Apply(this);
            }
        }

        public void SetNumericQuality(IHaveQualities entity, LowercaseString index, decimal toValue)
        {
            if (!this.DeltaValues.TryGetValue(entity, out QualitiesHolder overrideAttributes))
            {
                overrideAttributes = new QualitiesHolder();
                this.DeltaValues.Add(entity, new QualitiesHolder());
            }

            overrideAttributes.SetNumericQuality(index, toValue);
        }

        public decimal GetNumericQuality(IHaveQualities entity, LowercaseString index, decimal defaultValue = 0)
        {
            if (!this.DeltaValues.TryGetValue(entity, out QualitiesHolder overrideAttributes))
            {
                overrideAttributes = new QualitiesHolder();
                this.DeltaValues.Add(entity, new QualitiesHolder());
            }

            return overrideAttributes.GetNumericQuality(index, defaultValue);
        }

        public void ModCurrency(Currency toMod, int amount)
        {
            if (this.CurrencyChanges.TryGetValue(toMod, out int currentAmount))
            {
                this.CurrencyChanges[toMod] = currentAmount + amount;
            }
            else
            {
                this.CurrencyChanges.Add(toMod, amount);
            }
        }

        public int GetCurrency(Currency toMod)
        {
            int amount = this.BaseGameState.GetCurrency(toMod);
            if (this.CurrencyChanges.TryGetValue(toMod, out int mod))
            {
                amount += mod;
            }
            return amount;
        }

        public void RemoveEntity(Entity entity)
        {
            if (this.EntityDestinationChanges.ContainsKey(entity))
            {
                this.EntityDestinationChanges[entity] = EntityDestination.Removed;
            }
            else
            {
                this.EntityDestinationChanges.Add(entity, EntityDestination.Removed);
            }
        }

        public bool EntityIsAlive(Entity entity)
        {
            if (this.EntityDestinationChanges.TryGetValue(entity, out EntityDestination destination))
            {
                return destination != EntityDestination.Removed;
            }

            return this.BaseGameState.EntityIsAlive(entity);
        }

        public IReadOnlyList<Entity> GetAllEntities()
        {
            List<Entity> entities = new List<Entity>();
            entities.AddRange(this.BaseGameState.GetAllEntities());

            foreach (Entity movedEntity in this.EntityDestinationChanges.Keys)
            {
                switch (EntityDestinationChanges[movedEntity])
                {
                    case EntityDestination.Removed:
                        entities.Remove(movedEntity);
                        break;
                    case EntityDestination.Persistent:
                    case EntityDestination.Encounter:
                        if (!entities.Contains(movedEntity))
                        {
                            entities.Add(movedEntity);
                        }
                        break;
                }
            }

            return entities;
        }

        public void AddEncounterEntity(Entity toAdd)
        {
            if (this.EntityDestinationChanges.ContainsKey(toAdd))
            {
                this.EntityDestinationChanges[toAdd] = EntityDestination.Encounter;
            }
            else
            {
                this.EntityDestinationChanges.Add(toAdd, EntityDestination.Persistent);
            }
        }

        public void AddPersistentEntity(Entity toAdd)
        {
            if (this.EntityDestinationChanges.ContainsKey(toAdd))
            {
                this.EntityDestinationChanges[toAdd] = EntityDestination.Encounter;
            }
            else
            {
                this.EntityDestinationChanges.Add(toAdd, EntityDestination.Persistent);
            }
        }

        public void StartFactionTurn(decimal factionId)
        {
            this.NewFactionTurn = factionId;
            this.TriggerAndStack(new GameStateEventTrigger(Utility.Wellknown.WellknownGameStateEvents.FactionTurnStarted, GameStateEventTrigger.TriggerDirection.After));
        }

        public void StartEntityTurn(Entity toStart)
        {
            this.NewEntityTurn = toStart;
            this.TriggerAndStack(new GameStateEventTrigger(Utility.Wellknown.WellknownGameStateEvents.EntityTurnStarted, GameStateEventTrigger.TriggerDirection.After));
        }

        public void EndCurrentEntityTurn()
        {
            this.NewEntityTurn = null;
            this.TriggerAndStack(new GameStateEventTrigger(Utility.Wellknown.WellknownGameStateEvents.EntityTurnEnded, GameStateEventTrigger.TriggerDirection.After));
        }

        public void EndCurrentFactionTurn()
        {
            this.TriggerAndStack(new GameStateEventTrigger(Utility.Wellknown.WellknownGameStateEvents.FactionTurnEnded, GameStateEventTrigger.TriggerDirection.After));
        }

        public bool TryGetNextResolve(out IResolve currentResolve)
        {
            if (!this.PendingResolves.TryGetNextResolve(out currentResolve))
            {
                return false;
            }

            return true;
        }

        public void TriggerAndStack(GameStateEventTrigger trigger)
        {
            // First put on to the stack an instruction to resolve each status effect
            // We can grab all status effects with matching ids for trigger events
            // and then as they're resolving, can determine if anything should actually happen,
            // including whether or not the triggered ability actually triggers
            List<AppliedStatusEffect> possiblyTriggeredStatusEffects = new List<AppliedStatusEffect>();
            foreach (AppliedStatusEffect effect in this.GetAllStatusEffects())
            {
                if (effect.TriggerOnEventIds.Contains(trigger.EventId))
                {
                    possiblyTriggeredStatusEffects.Add(effect);
                }
            }
            possiblyTriggeredStatusEffects.Sort((a, b) => a.StatusEffectPriorityOrder.CompareTo(b.StatusEffectPriorityOrder));
            foreach (AppliedStatusEffect effect in possiblyTriggeredStatusEffects)
            {
                this.PendingResolves.Push(new ResolveTriggeredEvent(effect, trigger));
            }

            // Then put on top of the stack each rule in order of application
            // This way, rules always resolve before the triggered abilities consider triggering
            List<GameStateChange> appliedRules = RuleReference.GetAppliedRules(this, trigger);
            foreach (GameStateChange change in appliedRules)
            {
                this.PendingResolves.Push(change);
            }
        }

        public void MoveCard(CardInstance card, LowercaseString zone)
        {
            if (this.CardDestinationChanges.ContainsKey(card))
            {
                this.CardDestinationChanges[card] = zone;
            }
            else
            {
                this.CardDestinationChanges.Add(card, zone);
            }
        }

        public IReadOnlyList<CardInstance> GetCardsInZone(LowercaseString zone)
        {
            List<CardInstance> cardsInZone = new List<CardInstance>(this.BaseGameState.GetCardsInZone(zone));

            foreach (CardInstance movedCard in this.CardDestinationChanges.Keys)
            {
                if (this.CardDestinationChanges[movedCard] == zone)
                {
                    if (!cardsInZone.Contains(movedCard))
                    {
                        cardsInZone.Add(movedCard);
                    }
                }
                else
                {
                    cardsInZone.Remove(movedCard);
                }
            }

            return cardsInZone;
        }

        public void ShuffleDeck()
        {
            // TODO: Shuffle
        }

        public void AddCard(CardInstance card, LowercaseString zone)
        {
            this.CardDestinationChanges.Add(card, zone);
        }

        public IReadOnlyList<AppliedStatusEffect> GetAllStatusEffects()
        {
            throw new System.NotImplementedException();
        }

        public void PushResolve(IResolve toResolve)
        {
            throw new System.NotImplementedException();
        }

        public bool CanAfford(IEnumerable<IShopCost> costs)
        {
            throw new System.NotImplementedException();
        }

        public LowercaseString GetCardZone(CardInstance card)
        {
            throw new System.NotImplementedException();
        }

        public void SetStringQuality(IHaveQualities entity, LowercaseString index, string toValue)
        {
            throw new System.NotImplementedException();
        }

        public string GetStringQuality(IHaveQualities entity, LowercaseString index, string defaultValue = "")
        {
            throw new System.NotImplementedException();
        }

        public void ShuffleDiscardAndDeck()
        {
            throw new System.NotImplementedException();
        }

        public void ModStatusEffectStacks(Entity onEntity, LowercaseString statusEffectId, int modStacks)
        {
            throw new System.NotImplementedException();
        }

        public int GetStacks(Entity onEntity, LowercaseString statusEffectId)
        {
            throw new System.NotImplementedException();
        }

        public void ModifyElement(LowercaseString elementId, int modAmount)
        {
            throw new System.NotImplementedException();
        }
    }
}