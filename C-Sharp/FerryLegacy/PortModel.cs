using System;
using System.Collections.Generic;
using System.Linq;

namespace FerryLegacy
{
    public class PortModel
    {
        public int Id { get; private set; }

        private readonly Dictionary<int, TimeSpan> _ferryAvailability = new Dictionary<int, TimeSpan>();
        private readonly List<Ferry> _ferries = new List<Ferry>();

        public PortModel(Port port)
        {
            Id = port.Id;
        }

        public void AddFerry(TimeSpan available, Ferry ferry)
        {
            if (ferry != null)
            {
                _ferries.Add(ferry);
                _ferryAvailability.Add(ferry.Id, available);
            }
        }

        public Ferry GetNextAvailable(TimeSpan time)
        {
            var available = _ferryAvailability.FirstOrDefault(x => time >= x.Value);
            if (available.Key == 0) return null;
            _ferryAvailability.Remove(available.Key);
            var ferry = _ferries.Single(x => x.Id == available.Key);
            _ferries.Remove(ferry);
            return ferry;
        }
    }
}