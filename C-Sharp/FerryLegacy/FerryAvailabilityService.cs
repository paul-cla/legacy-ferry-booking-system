using System;
using System.Collections.Generic;
using System.Linq;

namespace FerryLegacy
{
    public class FerryAvailabilityService
    {
        private readonly TimeTables _timeTables;
        private Ferries _ferries;
        private Ports _ports;

        public FerryAvailabilityService(TimeTables timeTables, Ports ports, Ferries ferries)
        {
            _timeTables = timeTables;
            _ports = ports;
            _ferries = ferries;
        }

        public Ferry NextFerryAvailableFrom(int portId, TimeSpan time)
        {
            var ports1 = _ports.All().Select(x => new PortModel(x)).ToList();
            foreach (var ferry in _ferries.All())
            {
                var port = ports1.Single(x => x.Id == ferry.HomePortId);
                port.AddFerry(new TimeSpan(0, 0, 0), ferry);
            }
            var ports = ports1;
            var allEntries = _timeTables.All().SelectMany(x => x.Entries).OrderBy(x => x.Time).ToList();

            foreach (var entry in allEntries)
            {
                var journey = new Journey
                {
                    Origin = ports.Single(x => x.Id == entry.OriginId),
                    Destination = ports.Single(x => x.Id == entry.DestinationId)
                };

                journey.Ferry = journey.Origin.GetNextAvailable(entry.Time);
                
                var destination = journey.Destination;

                if (destination == null)
                    throw new ArgumentNullException(nameof(destination));

                var arrivalTime = entry.Time.Add(entry.JourneyTime);
                int result;
                if (destination.Id == 3)
                    result = 25;
                else if (destination.Id == 2)
                    result = 20;
                else
                    result = 15;
                var turnaroundTime = result;
                var timeReady = arrivalTime.Add(TimeSpan.FromMinutes(turnaroundTime));
                var time1 = timeReady;

                destination.AddFerry(time1, journey.Ferry);
                if (entry.OriginId == portId && entry.Time >= time)
                {
                    return journey.Ferry;
                }
            }

            return null;
        }
    }
}
