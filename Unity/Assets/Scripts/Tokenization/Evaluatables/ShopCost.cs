using SpaceDeck.GameState.Minimum;
using SpaceDeck.Tokenization.Evaluatables;
using SpaceDeck.Utility.Logging;
using SpaceDeck.Utility.Wellknown;

namespace SpaceDeck.Tokenization.Evaluatables
{
    public class ShopCost : IShopCost
    {
        public Currency CurrencyType => this._CurrencyType;
        private readonly Currency _CurrencyType;

        public IEvaluatableValue<int> CostAmount;

        public ShopCost(Currency currencyType, IEvaluatableValue<int> costAmount)
        {
            this._CurrencyType = currencyType;
            this.CostAmount = costAmount;
        }

        public int GetCost(IGameStateMutator mutator)
        {
            if (!this.CostAmount.TryEvaluate(mutator, out int value))
            {
                Logging.DebugLog(WellknownLoggingLevels.Error, WellknownLoggingCategories.EvaluatableEvaluation, $"Could not evaluate cost amount.");
            }

            return value;
        }
    }
}