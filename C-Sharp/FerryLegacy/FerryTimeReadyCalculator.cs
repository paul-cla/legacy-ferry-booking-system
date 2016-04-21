using System;

namespace FerryLegacy
{
    public static class FerryTimeReadyCalculator
    {
        public static TimeSpan TimeReady(TimeTableEntry timetable, PortModel destination)
        {
            if (timetable == null)
                return new TimeSpan(0,0,0);
            if (destination == null)
                throw new ArgumentNullException(nameof(destination));

            var arrivalTime = timetable.Time.Add(timetable.JourneyTime);
            var turnaroundTime = JourneyManager.GetFerryTurnaroundTime(destination);
            var timeReady = arrivalTime.Add(TimeSpan.FromMinutes(turnaroundTime));
            return timeReady;
        }
    }
}