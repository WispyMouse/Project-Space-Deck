namespace SpaceDeck.GameState.Minimum
{
    public interface IShopCost
    {
        int GetCost(IGameStateMutator mutator);
        Currency CurrencyType { get; }
    }
}