using System.Collections.Generic;
using System.Linq;

namespace ShipRouting
{
    public class RoutingService
    {
        private readonly IRouteRepository _routeRepository;
        private readonly List<RouteNetworkConfiguration> _routeConfigurations; 

        public RoutingService(IRouteRepository routeRepository)
        {
            _routeRepository = routeRepository;
            _routeConfigurations = _routeRepository.Get();
        }
        public RouteSummaryDetails GetRouteSummaryDetails(List<string> routes)
        {
            var routeConfigurations = _routeRepository.Get();
            var result = new RouteSummaryDetails();
            for (var route = 1; route < routes.Count; route++)
            {
                if (route < routes.Count)
                {
                    var desiredRoute = routeConfigurations.FirstOrDefault(_ => _.EndDestination == routes[route] && 
                        _.StartDestination == routes[route - 1]);
                    if (desiredRoute == null)
                    {
                        result.RouteIsValid = false;
                        result.JourneyTime = 0;
                        return result;
                    }

                    if (desiredRoute.StartDestination == routes[route - 1])
                    {
                        result.JourneyTime += desiredRoute.JourneyTimes;
                        result.RouteIsValid = true;
                    }
                }
            }

            return result;
            
        }
        public int GetShortestJourneyTime(string startDestination, string endDestination)
        {
            var routes = Get(startDestination, endDestination);
           
            return routes.Values.Min(x => x.Sum(_ => _.JourneyTimes));
        }

        public int GetNumberOfRoutesWithinAMaximumNumberOfStops(string startDestination, string endDestination,
            int maximumNuberOfStops)
        {
            return Get(startDestination, endDestination).Count(_ => _.Value.Count <= maximumNuberOfStops);
        }

        public int GetNumberOfRoutesWhereExactlyANumberOfStopsAreMade(string startDestination, string endDestination,
            int numberOfStops)
        {
            return Get(startDestination, endDestination).Count(_ => _.Value.Count == numberOfStops);
        }
        
        public int GetNumberOfRoutesWhereJourneyTimeIsWithinspecifiedDays(string startDestination, string endDestination,
            int numberOfDays)
        {
            var result = Get(startDestination, endDestination).Values.Count(_ => _.Sum(x => x.JourneyTimes) <= numberOfDays);
            return result;
        }
        private Dictionary<int, List<RouteNetworkConfiguration>> Get(string startDestination, string endDestination)
        {
            var result = new Dictionary<int, List<RouteNetworkConfiguration>>();
            var currentIndex = 0;
            var queueOfRoutes = PopulateQueueOfRoutes(startDestination);
           
            while (queueOfRoutes.Count > 0)
            {
                var routeToAdd = queueOfRoutes.Pop();
                routeToAdd = AddRoutes(startDestination, result, currentIndex, routeToAdd);

                if (routeToAdd!=null && routeToAdd.EndDestination != endDestination)
                {
                    var routes = _routeConfigurations.Where(_ => _.StartDestination == routeToAdd.EndDestination).ToList();
                    routes.ForEach(_ => queueOfRoutes.Push(_));
                }
                if (routeToAdd != null && routeToAdd.EndDestination == endDestination)
                {
                    currentIndex++;
                }
            }
            return result;
        }

        private RouteNetworkConfiguration AddRoutes(string startDestination, Dictionary<int, List<RouteNetworkConfiguration>> result, int currentIndex,
            RouteNetworkConfiguration routeToAdd)
        {
            if (!result.ContainsKey(currentIndex))
            {
                if (routeToAdd.StartDestination != startDestination)
                {
                    var startRoute =
                        _routeConfigurations.FirstOrDefault(_ => _.StartDestination == startDestination &&
                                                                 _.EndDestination == routeToAdd.StartDestination);
                    if (startRoute != null)
                    {
                        result.Add(currentIndex, new List<RouteNetworkConfiguration> {startRoute});
                        result[currentIndex].Add(routeToAdd);
                    }
                    else
                    {
                        routeToAdd = null;
                    }
                }
                else result.Add(currentIndex, new List<RouteNetworkConfiguration> {routeToAdd});
            }
            else result[currentIndex].Add(routeToAdd);
            return routeToAdd;
        }

        private Stack<RouteNetworkConfiguration> PopulateQueueOfRoutes(string startDestination)
        {
            var queueOfRoutes = new Stack<RouteNetworkConfiguration>();
            var routes = _routeConfigurations.Where(_ => _.StartDestination == startDestination).ToList();
            routes.ForEach(_ => queueOfRoutes.Push(_));
            return queueOfRoutes;
        }
    }
}
