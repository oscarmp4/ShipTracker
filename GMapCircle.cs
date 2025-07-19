using System;
using System.Collections.Generic;
using System.Drawing;
using GMap.NET;
using GMap.NET.WindowsForms;

public class GMapCircle : GMapPolygon
{
    public double Radius { get; set; }
    public PointLatLng Position { get; set; }

    public GMapCircle(PointLatLng center, double radius) : base(CreateCirclePoints(center, radius), "circle")
    {
        this.Radius = radius;
        this.Position = center;
    }



    public static List<PointLatLng> CreateCirclePoints(PointLatLng center, double radius)
    {
        int segments = 100;
        var points = new List<PointLatLng>();

        for (int i = 0; i < segments; i++)
        {
            double theta = (double)i / segments * 2 * Math.PI;
            double dx = radius * Math.Cos(theta);
            double dy = radius * Math.Sin(theta);

            var point = Offset(center, dx, dy);
            points.Add(point);
        }

        return points;
    }

    private static PointLatLng Offset(PointLatLng center, double east, double north)
    {
        double earthRadius = 6378137; // meters

        double dLat = north / earthRadius;
        double dLng = east / (earthRadius * Math.Cos(Math.PI * center.Lat / 180));

        double lat = center.Lat + dLat * 180 / Math.PI;
        double lng = center.Lng + dLng * 180 / Math.PI;

        return new PointLatLng(lat, lng);
    }
}
