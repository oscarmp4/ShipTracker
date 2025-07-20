using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using ShipTracker.Models;

namespace ShipTracker
{
    public static class DAL
    {
        private static readonly string connectionString =
            "Data Source=SQLDESKTOP;Initial Catalog=ShipTracker;User ID=xx;Password=xxxxx;";

        public static Route GetRouteForShip(int shipId)
        {
            Route route = null;
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"
                    SELECT r.ShipId, p1.PortName AS OriginPort, p1.Latitude AS OriginLat, p1.Longitude AS OriginLon,
                           p2.PortName AS DestPort, p2.Latitude AS DestLat, p2.Longitude AS DestLon, r.ETA
                    FROM Routes r
                    JOIN Ports p1 ON r.OriginPortId = p1.PortId
                    JOIN Ports p2 ON r.DestinationPortId = p2.PortId
                    WHERE r.ShipId = @ShipId", conn);
                cmd.Parameters.AddWithValue("@ShipId", shipId);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        route = new Route
                        {
                            ShipId = reader.GetInt32(0),
                            OriginPort = reader.GetString(1),
                            OriginLatitude = reader.GetDouble(2),
                            OriginLongitude = reader.GetDouble(3),
                            DestinationPort = reader.GetString(4),
                            DestinationLatitude = reader.GetDouble(5),
                            DestinationLongitude = reader.GetDouble(6),
                            ETA = reader.GetDateTime(7)
                        };
                    }
                }
            }
            return route;
        }

        public static List<Position> GetPositionHistoryForShip(int shipId, int limit = 10)
        {
            var positions = new List<Position>();
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"
                    SELECT TOP (@Limit) Latitude, Longitude, Timestamp
                    FROM Positions
                    WHERE ShipId = @ShipId
                    ORDER BY Timestamp DESC", conn);
                cmd.Parameters.AddWithValue("@ShipId", shipId);
                cmd.Parameters.AddWithValue("@Limit", limit);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        positions.Add(new Position
                        {
                            Latitude = reader.GetDouble(0),
                            Longitude = reader.GetDouble(1),
                            Timestamp = reader.GetDateTime(2)
                        });
                    }
                }
            }
            return positions;
        }

        public static List<Port> LoadPortsFromDatabase()
        {
            var ports = new List<Port>();
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT PortId, PortName, Country, Latitude, Longitude FROM Ports", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ports.Add(new Port
                        {
                            PortId = reader.GetInt32(0),
                            PortName = reader.GetString(1),
                            Country = reader.GetString(2),
                            Latitude = reader.GetDouble(3),
                            Longitude = reader.GetDouble(4)
                        });
                    }
                }
            }
            return ports;
        }
    }
}
