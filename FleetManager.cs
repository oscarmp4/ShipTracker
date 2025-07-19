using System.Collections.Generic;

namespace ShipTracker
{
    public class FleetManager
    {
        public List<Ship> Ships { get; set; } = new List<Ship>();
    }

    public class Ship
    {        
        public string Name { get; set; }
        public string IMO { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Status { get; set; }
        public int ShipId { get; internal set; }
        public string MMSI { get; internal set; }
        public string VesselType { get; internal set; }
        public object Heading { get; internal set; }
    }
}
