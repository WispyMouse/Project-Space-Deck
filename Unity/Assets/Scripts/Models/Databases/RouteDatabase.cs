namespace SpaceDeck.Models.Databases
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.Models.Imports;
    using SpaceDeck.Models.Instances;
    using SpaceDeck.Models.Prototypes;
    using SpaceDeck.Utility.Logging;
    using SpaceDeck.Utility.Minimum;
    using SpaceDeck.Utility.Wellknown;

    public static class RouteDatabase
    {
        public static List<Route> AllRoutes = new List<Route>();

        public static void AddRouteToDatabase(RouteImport toAdd)
        {
            if (toAdd == null)
            {
                Logging.DebugLog(WellknownLoggingLevels.Error,
                    WellknownLoggingCategories.Route,
                    $"{nameof(RouteImport)} is null.");
                return;
            }

            Route route = toAdd.GetRoute();

            if (route == null)
            {
                Logging.DebugLog(WellknownLoggingLevels.Error,
                    WellknownLoggingCategories.Route,
                    $"({toAdd.Id}) Generated route from {nameof(RouteImport)} is null.");
                return;
            }

            AddRouteToDatabase(route);
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