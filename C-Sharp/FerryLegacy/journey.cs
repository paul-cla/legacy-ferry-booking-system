using System;

namespace FerryLegacy
{
    public class Journey
    {
        public Ferry Ferry { get; set; }

        public PortModel Origin { get; set; }
        public PortModel Destination { get; set; }
    }
}