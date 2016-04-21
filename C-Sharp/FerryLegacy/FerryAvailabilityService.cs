using System;
using System.Collections.Generic;
using System.Linq;

namespace FerryLegacy
{
    public class FerryAvailabilityService
    {
        private readonly TimeTables _timeTables;
        private readonly PortManager _portManager;

        public FerryAvailabilityService(TimeTables timeTables, PortManager portManager)
        {
            _timeTables = timeTables;
            _portManager = portManager;
        }

        public Ferry NextFerryAvailableFrom(int portId, TimeSpan time)
        {
            var ports = _portManager.PortModels();
            var allEntries = _timeTables.All().SelectMany(x => x.Entries).OrderBy(x => x.Time).ToList();

            foreach (var entry in allEntries)
            {
                var journey = new Journey
                {
                    Origin = ports.Single(x => x.Id == entry.OriginId),
                    Destination = ports.Single(x => x.Id == entry.DestinationId)
                };

                var destination = journey.Destination;
                if (journey.Ferry == null)
                {
                    journey.Ferry = journey.Origin.GetNextAvailable(entry.Time);
                }

                var ferry = journey.Ferry;

                var time1 = FerryTimeReadyCalculator.TimeReady(entry, destination);
                destination.AddFerry(time1, ferry);
                if (entry.OriginId == portId && entry.Time >= time)
                {
                    return journey.Ferry;
                }
            }

            return null;
        }
    }
}
