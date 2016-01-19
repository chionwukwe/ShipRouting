using System.Collections.Generic;

namespace ShipRouting
{
    public interface IRouteRepository
    {
        void AddRoute(RouteNetworkConfiguration routeDetails);
        List<RouteNetworkConfiguration> Get();
    }
}
