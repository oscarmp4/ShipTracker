using System;

namespace ShipTracker.Models
{
    public class Ship
    {
        public int ShipId { get; set; }      // 
        public string Name { get; set; }
        public string IMO { get; set; }
        public string MMSI { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Status { get; set; }
        public string VesselType { get; set; }  // e.g., "Cargo", "Tanker"
        public double Heading { get; set; }     // in degrees, 0-359

    }
}
