using System;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.Projections;

public class OpenSeaMapChartProvider : GMapProvider
{
    public static readonly OpenSeaMapChartProvider Instance = new OpenSeaMapChartProvider();

    public override Guid Id => new Guid("b7b27e32-3e4a-4a59-91f2-9fb1f9c11023");

    public override string Name => "OpenSeaMapChart";

    public override PureProjection Projection => MercatorProjection.Instance;

    public override GMapProvider[] Overlays => new GMapProvider[] { OpenStreetMapProvider.Instance, this };

    public override PureImage GetTileImage(GPoint pos, int zoom)
    {
        string url = MakeTileImageUrl(pos, zoom, LanguageStr);
        return GetTileImageUsingHttp(url);
    }

    string MakeTileImageUrl(GPoint pos, int zoom, string language)
    {
        return $"https://tiles.openseamap.org/seamark/{zoom}/{pos.X}/{pos.Y}.png";
    }
}
