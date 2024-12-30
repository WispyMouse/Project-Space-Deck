namespace SpaceDeck.Models.Imports
{
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Models.Prototypes;
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.Utility.Minimum;
    using System.Collections;
    using System.Collections.Generic;

    [System.Serializable]
    public class RouteImport : Importable
    {
        public string Name;

        public Route GetRoute()
        {
            return new Route(this.Name);
        }
    }
}