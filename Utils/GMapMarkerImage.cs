using System.Drawing;
using GMap.NET;
using GMap.NET.WindowsForms;

public class GMapMarkerImage : GMapMarker
{
    private readonly Bitmap icon;

    public GMapMarkerImage(PointLatLng p, Bitmap iconImage) : base(p)
    {
        icon = iconImage;
    }

    public override void OnRender(Graphics g)
    {
        if (icon != null)
        {
            int iconSize = 24; // set the desired icon size here
            var resizedIcon = new Bitmap(icon, new Size(iconSize, iconSize));
            g.DrawImage(resizedIcon, LocalPosition.X - iconSize / 2, LocalPosition.Y - iconSize / 2, iconSize, iconSize);
        }
    }
}
