using System;

namespace ShipTracker
{
    internal class GeoCoordinate
    {
        private double lat;
        private double lng;

        public GeoCoordinate(double lat, double lng)
        {
            this.lat = lat;
            this.lng = lng;
        }

        internal int GetDistanceTo(GeoCoordinate eCoord)
        {
            throw new NotImplementedException();
        }
    }
}