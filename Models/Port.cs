using System;

namespace ShipTracker.Models
{
    public class Port
    {
        public int PortId { get; set; }
        public string PortName { get; set; }
        public string Country { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
