using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;

namespace ShipRouting.Tests
{
    [TestFixture]
    public class RoutingServiceTests
    {
        private List<RouteNetworkConfiguration> _routeNetworkConfigurations;
        private IRouteRepository _routeRepository;
        private RoutingService _routingService;
            
        [SetUp]
        public void Initialize()
        {
            _routeNetworkConfigurations = new List<RouteNetworkConfiguration>
            {
                new RouteNetworkConfiguration(DestinationName.BuenosAires, DestinationName.NewYork, 6),
                new RouteNetworkConfiguration(DestinationName.BuenosAires, DestinationName.Casablanca, 5),
                new RouteNetworkConfiguration(DestinationName.BuenosAires, DestinationName.CapeTown, 4),
                new RouteNetworkConfiguration(DestinationName.NewYork, DestinationName.Liverpool, 4),
                new RouteNetworkConfiguration(DestinationName.Liverpool, DestinationName.Casablanca, 3),
                new RouteNetworkConfiguration(DestinationName.Liverpool, DestinationName.CapeTown, 6),
                new RouteNetworkConfiguration(DestinationName.Casablanca, DestinationName.Liverpool, 3),
                new RouteNetworkConfiguration(DestinationName.Casablanca, DestinationName.CapeTown, 6),
                new RouteNetworkConfiguration(DestinationName.CapeTown, DestinationName.NewYork, 8)
            };
            var mockRouteRepository = new Mock<IRouteRepository>();
            mockRouteRepository.Setup(_ => _.Get()).Returns(_routeNetworkConfigurations);  
            _routeRepository = mockRouteRepository.Object;

            _routingService = new RoutingService(_routeRepository);
        }

        [TestCase(10, true, DestinationName.BuenosAires, DestinationName.NewYork, DestinationName.Liverpool)]
        [TestCase(8, true, DestinationName.BuenosAires, DestinationName.Casablanca, DestinationName.Liverpool)]
        [TestCase(19, true, DestinationName.BuenosAires, DestinationName.CapeTown, DestinationName.NewYork, DestinationName.Liverpool, DestinationName.Casablanca)]
        [TestCase(0, false, DestinationName.BuenosAires, DestinationName.CapeTown, DestinationName.Casablanca)]
        public void GivenARoute_ShouldReturnExpectedJourneyTime(int expectedJourneyTime, bool expectedRouteIsValid, params string[] destinations)
        {
            var routes = destinations.ToList();

            var summaryDetails = _routingService.GetRouteSummaryDetails(routes);

            Assert.AreEqual(expectedRouteIsValid, summaryDetails.RouteIsValid);
            Assert.AreEqual(expectedJourneyTime, summaryDetails.JourneyTime);
        }
        
        [TestCase(DestinationName.BuenosAires, DestinationName.Liverpool, 8)]
        [TestCase(DestinationName.NewYork, DestinationName.NewYork, 18)]
        public void GivenAStartRouteToEndDestination_ShouldReturnShortestJourneyTime(string startDestination, string endDestination, int expectedShortestJourneyTime)
        {
            var journeyTime = _routingService.GetShortestJourneyTime(startDestination, endDestination);
            
            Assert.AreEqual(expectedShortestJourneyTime, journeyTime);
        }

        [TestCase(DestinationName.Liverpool, DestinationName.Liverpool, 3, 2)]
        [TestCase(DestinationName.BuenosAires, DestinationName.Liverpool, 2, 2)]
        [TestCase(DestinationName.NewYork, DestinationName.Liverpool, 1, 1)]
        public void GivenARoute_ReturnNumberOfRoutesThatCanBeTaken(string startDestination, string endDestination, int maximumNumberOfStops, int expectedNumberOfRoutes)
        {
            var numberOfRoutes = _routingService.GetNumberOfRoutesWithinAMaximumNumberOfStops(startDestination, endDestination, maximumNumberOfStops);

            Assert.AreEqual(expectedNumberOfRoutes, numberOfRoutes);
        }

        [TestCase(DestinationName.BuenosAires, DestinationName.Liverpool, 4, 1)]
        [TestCase(DestinationName.NewYork, DestinationName.NewYork, 3, 1)]
        public void GivenARoute_ReturnNumberOfRoutesWithExactlyFourStop(string startDestination, string endDestination, int numberOfStopsMade, int expectedResult)
        {
            var numberOfRoutes = _routingService.GetNumberOfRoutesWhereExactlyANumberOfStopsAreMade(startDestination, endDestination, numberOfStopsMade);

            Assert.AreEqual(expectedResult, numberOfRoutes);
        }

        [Test]
        public void GivenARoute_ReturnNumberOfRoutesWhereJourneyTimeIsLessThanOrEqualToSpecifiedNumberOfDays()
        {
            var result = _routingService.GetNumberOfRoutesWhereJourneyTimeIsWithinspecifiedDays(DestinationName.Liverpool, 
                                                            DestinationName.Liverpool, 25);

            Assert.AreEqual(3, result);
        }
        
    }
}
