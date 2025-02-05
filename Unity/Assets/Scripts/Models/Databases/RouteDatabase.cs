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

        /// <summary>
        /// Links all routes, making each evaluatable and token have proper references.
        /// Unlike other databases, this linking will randomize the results of things with arguments.
        /// Re-Linking this database will create a new seed for a run.
        /// </summary>
        public static void LinkAllRoutes()
        {
            foreach (Route curRoute in AllRoutes)
            {
                 foreach (ChoiceNode node in curRoute.Choices)
                {
                    foreach (ChoiceNodeOption option in node.Options)
                    {
                        if (!EncounterDatabase.TryGetEncounterWithArguments(null, option.WillEncounterId, null, out EncounterInstance encounter))
                        {
                            Logging.DebugLog(WellknownLoggingLevels.Error,
                                WellknownLoggingCategories.LinkingFailure,
                                $"Could not link encounter. Id: '{option.WillEncounterId}'");
                            continue;
                        }

                        option.WillEncounter = encounter;
                    }
                }
            }
        }
    }
}