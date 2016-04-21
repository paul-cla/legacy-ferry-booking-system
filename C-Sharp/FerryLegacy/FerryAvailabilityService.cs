using System;
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
                var ferry = JourneyManager.CreateJourney(ports, entry);
                FerryReady(entry, ferry.Destination, ferry);
                if (entry.OriginId == portId && entry.Time >= time)
                {
                    return ferry.Ferry;
                }
            }

            return null;
        }

        private static void FerryReady(TimeTableEntry timetable, PortModel destination, Journey journey)
        {
            if (journey.Ferry == null)
                JourneyManager.AddFerry(timetable, journey);

            var ferry = journey.Ferry;

            var time = FerryTimeReadyCalculator.TimeReady(timetable, destination);
            destination.AddFerry(time, ferry);
        }
    }
}
