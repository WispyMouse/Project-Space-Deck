namespace SpaceDeck.GameState.Minimum
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using SpaceDeck.Utility.Minimum;
    using static SpaceDeck.GameState.Minimum.GameStateEventTrigger;

    public interface IGameStateMutator
    {
        EntityTurnTakerCalculator EntityTurnTakerCalculator { get; set; }
        FactionTurnTakerCalculator FactionTurnTakerCalculator { get; set; }

        void SetNumericQuality(IHaveQualities entity, LowercaseString index, decimal toValue);
        decimal GetNumericQuality(IHaveQualities entity, LowercaseString index, decimal defaultValue = 0);
        int GetCurrency(Currency currency);
        void ModCurrency(Currency currency, int modAmount);
        void SetStringQuality(IHaveQualities entity, LowercaseString index, string toValue);
        string GetStringQuality(IHaveQualities entity, LowercaseString index, string defaultValue = "");

        void SetIntensity(IChangeWithIntensity intensity, decimal newValue);
        decimal GetIntensity(IChangeWithIntensity intensity);

        void AddEncounterEntity(Entity toAdd);
        void AddPersistentEntity(Entity toAdd);
        void RemoveEntity(Entity entity);
        bool EntityIsAlive(Entity entity);
        IReadOnlyList<Entity> GetAllEntities();

        IReadOnlyList<AppliedStatusEffect> GetAllStatusEffects();
        void ModStatusEffectStacks(Entity onEntity, LowercaseString statusEffectId, int modStacks);
        int GetStacks(Entity onEntity, LowercaseString statusEffectId);
        void ModifyElement(Element element, int modAmount);
        int GetElement(Element element);

        void StartFactionTurn(decimal factionId);
        void EndCurrentFactionTurn();

        void StartEntityTurn(Entity toStart);
        void EndCurrentEntityTurn();

        IReadOnlyList<IResolve> GetTriggers(GameStateEventTrigger trigger, TriggerDirection direction);
        void TriggerAndStack(GameStateEventTrigger trigger);
        bool TryGetNextResolve(out IResolve currentResolve);
        void PushResolve(IResolve toResolve);

        void MoveCard(CardInstance card, LowercaseString zone);
        IReadOnlyList<CardInstance> GetCardsInZone(LowercaseString zone);
        LowercaseString GetCardZone(CardInstance card);
        void ShuffleDeck();
        void ShuffleDiscardAndDeck();
        void AddCard(CardInstance card, LowercaseString zone);

        bool CanAfford(IEnumerable<IShopCost> costs);
    }
}