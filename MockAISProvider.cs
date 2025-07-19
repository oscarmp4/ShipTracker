using System;
using System.Collections.Generic;
using ShipTracker.Models;


namespace ShipTracker
{
    public static class MockAISProvider
    {
        public static List<Ship> GetShips()
        {
            return new List<Ship>//Lat: 23.1330592107206, Lng: -82.3282814025879
    {
        new Ship { ShipId = 2, Name = "Ocean Queen", IMO = "IMO2345678", MMSI = "987654321", VesselType = "Passenger", Latitude = 23.1330, Longitude = -82.3282, Heading = 45, Status = "In Port" },
        new Ship { ShipId = 3, Name = "Tanker Max", IMO = "IMO3456789", MMSI = "555666777", VesselType = "Tanker", Latitude = 35.6895, Longitude = 139.6917, Heading = 180, Status = "At Sea" },
        new Ship { ShipId = 4, Name = "Fishing Spirit", IMO = "IMO4567890", MMSI = "111222333", VesselType = "Fishing", Latitude = 34.0522, Longitude = -118.2437, Heading = 270, Status = "At Sea" },
        // Add more ships as needed...
    };
        }

    }
}
