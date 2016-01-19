namespace ShipRouting
{
    public class RouteNetworkConfiguration
    {
        public RouteNetworkConfiguration(string startDestination, string endDestination, int journeyTimes)
        {
            StartDestination = startDestination;
            EndDestination = endDestination;
            JourneyTimes = journeyTimes;
        }

        public string StartDestination { get; private set; }
        public string EndDestination { get; private set; }
        public int JourneyTimes { get; private set; }
    }
}
