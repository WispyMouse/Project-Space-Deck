namespace SpaceDeck.Models.Databases
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Models.Imports;
    using SpaceDeck.Models.Instances;
    using SpaceDeck.Models.Prototypes;
    using SpaceDeck.Utility.Minimum;

    public static class RouteDatabase
    {
        public static List<Route> AllRoutes = new List<Route>();

        public static void AddRouteToDatabase(RouteImport toAdd)
        {
            AddRouteToDatabase(toAdd.GetRoute());
        }

        public static void AddRouteToDatabase(Route toAdd)
        {
            AllRoutes.Add(toAdd);
        }

        public static void ClearDatabase()
        {
            AllRoutes.Clear();
        }
    }
}