using System.Collections.Generic;
using System.Linq;

namespace FerryLegacy
{
    public static class JourneyManager
    {
        public static Journey CreateJourney(List<PortModel> ports, TimeTableEntry timetable)
        {
            return new Journey
            {
                Origin = ports.Single(x => x.Id == timetable.OriginId),
                Destination = ports.Single(x => x.Id == timetable.DestinationId)
            };
        }

        public static void AddFerry(TimeTableEntry timetable, Journey journey)
        {
            journey.Ferry = journey.Origin.GetNextAvailable(timetable.Time);
        }

        public static int GetFerryTurnaroundTime(PortModel destination)
        {
            if (destination.Id == 3)
                return 25;
            if (destination.Id == 2)
                return 20;
            return 15;
        }
    }
}