using System;

namespace ShipTracker.Models
{
    public class Route
    {
        public int ShipId { get; set; }
        public string OriginPort { get; set; }
        public string DestinationPort { get; set; }
        public double OriginLatitude { get; set; }
        public double OriginLongitude { get; set; }
        public double DestinationLatitude { get; set; }
        public double DestinationLongitude { get; set; }
        public DateTime ETA { get; set; }
    }
}
