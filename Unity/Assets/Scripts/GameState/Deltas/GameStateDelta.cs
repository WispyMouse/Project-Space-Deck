namespace SpaceDeck.GameState.Deltas
{
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Utility.Minimum;
    using System.Collections;
    using System.Collections.Generic;
    using static SpaceDeck.GameState.Minimum.GameStateEventTrigger;

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
        public readonly Dictionary<IChangeWithIntensity, decimal> IntensityChanges = new Dictionary<IChangeWithIntensity, decimal>();
        public readonly Dictionary<Element, decimal> ElementChanges = new Dictionary<Element, decimal>();

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
            this.TriggerAndStack(new GameStateEventTrigger(Utility.Wellknown.WellknownGameStateEvents.FactionTurnStarted));
        }

        public void StartEntityTurn(Entity toStart)
        {
            this.NewEntityTurn = toStart;
            this.TriggerAndStack(new GameStateEventTrigger(Utility.Wellknown.WellknownGameStateEvents.EntityTurnStarted));
        }

        public void EndCurrentEntityTurn()
        {
            this.NewEntityTurn = null;
            this.TriggerAndStack(new GameStateEventTrigger(Utility.Wellknown.WellknownGameStateEvents.EntityTurnEnded));
        }

        public void EndCurrentFactionTurn()
        {
            this.TriggerAndStack(new GameStateEventTrigger(Utility.Wellknown.WellknownGameStateEvents.FactionTurnEnded));
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
            // The order of resolution should be:
            // - "Before" triggered effects, that are responding to an event being triggered
            // - If there is a GameStateChange to apply, apply it here
            // - "After" triggered effects
            // So that means we stack "after" first
            this.PushResolve(new TriggerAndResolve(trigger, TriggerDirection.After));

            if (trigger.BasedOnGameStateChange != null)
            {
                this.PushResolve(trigger.BasedOnGameStateChange);
            }

            this.PushResolve(new TriggerAndResolve(trigger, TriggerDirection.Before));
        }

        public IReadOnlyList<IResolve> GetTriggers(GameStateEventTrigger trigger, TriggerDirection direction)
        {
            List<IResolve> triggeredResolves = new List<IResolve>();

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
                triggeredResolves.Add(new ResolveTriggeredEvent(effect, trigger, direction));
            }

            // Then put on top of the stack each rule in order of application
            // This way, rules always resolve before the triggered abilities consider triggering
            List<GameStateChange> appliedRules = RuleReference.GetAppliedRules(this, trigger);
            foreach (GameStateChange change in appliedRules)
            {
                triggeredResolves.Add(change);
            }

            return triggeredResolves;
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
            List<AppliedStatusEffect> statusEffects = new List<AppliedStatusEffect>();

            foreach (Entity curEntity in this.GetAllEntities())
            {
                foreach (AppliedStatusEffect effect in curEntity.AppliedStatusEffects.Values)
                {
                    statusEffects.Add(effect);
                }
            }

            return statusEffects;
        }

        public void PushResolve(IResolve toResolve)
        {
            this.PendingResolves.Push(toResolve);
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
            // TODO
            return this.BaseGameState.GetStacks(onEntity, statusEffectId);
        }

        public void ModifyElement(Element element, int modAmount)
        {
            int newValue = GetElement(element) + modAmount;

            // GetElement will certainly result in the creation of an entry in
            // ElementChanges for this element, but just in case, this is safe
            if (this.ElementChanges.TryGetValue(element, out decimal currentValue))
            {
                this.ElementChanges[element] = currentValue + modAmount;
            }
            else
            {
                this.ElementChanges.Add(element, modAmount);
            }
        }

        public int GetElement(Element toGet)
        {
            if (this.ElementChanges.TryGetValue(toGet, out decimal value))
            {
                return (int)value;
            }

            return this.BaseGameState.GetElement(toGet);
        }

        public void SetIntensity(IChangeWithIntensity intensity, decimal newValue)
        {
            if (this.IntensityChanges.ContainsKey(intensity))
            {
                this.IntensityChanges[intensity] = newValue;
            }
            else
            {
                this.IntensityChanges.Add(intensity, newValue);
            }
        }

        public decimal GetIntensity(IChangeWithIntensity intensity)
        {
            if (this.IntensityChanges.TryGetValue(intensity, out decimal value))
            {
                return value;
            }

            return intensity.Intensity;
        }
    }
}