using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using ShipTracker.Models;

namespace ShipTracker
{
    public partial class MainForm : Form
    {
        private GMapOverlay shipsOverlay = new GMapOverlay("ships");
        private FleetManager fleetManager = new FleetManager();
        private PointLatLng? distanceStartPoint = null;
        private GMapRoute distanceLine;
        private GMapCircle distanceCircle;
        private bool isMeasuringDistance = false;
        private ToolStripStatusLabel latLabel;
        private ToolStripStatusLabel lonLabel;
        private TrackBar brightnessTrackBar;



        public MainForm()
        {
            InitializeComponent();

            brightnessTrackBar.Scroll += (s, e) =>
            {
                int brightness = brightnessTrackBar.Value;
                float factor = brightness / 100f;

                this.Opacity = factor; // dim entire window

                // Alternative: apply dim overlay to only the map if you want
            };


            this.Load += MainForm_Load;
            gmapControl.OnMarkerClick += Gmap_OnMarkerClick;
            gmapControl.OnMapZoomChanged += Gmap_OnMapZoomChanged;
            gmapControl.MouseDown += GmapControl_MouseDown;
            gmapControl.MouseMove += GmapControl_MouseMove;
            gmapControl.MouseUp += GmapControl_MouseUp;


            SetupMap();
            LoadMockShips();
            ShowPortsOnMap();
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            SetupMapContextMenu();
        }
        private void Gmap_OnMapZoomChanged()
        {
            var portsOverlay = gmapControl.Overlays.FirstOrDefault(o => o.Id == "ports");
            if (portsOverlay != null)
            {
                portsOverlay.IsVisibile = gmapControl.Zoom >= 6;  // show anchors only on zoom 10+
            }
        }


        private void SetupMap()
        {
            gmapControl.MapProvider = GMapProviders.GoogleMap;
            GMaps.Instance.Mode = AccessMode.ServerAndCache;
            gmapControl.Position = new PointLatLng(0, 0);
            gmapControl.MinZoom = 1;
            gmapControl.MaxZoom = 20;
            gmapControl.Zoom = 2;
            gmapControl.CanDragMap = true;
            gmapControl.DragButton = MouseButtons.Left;
            gmapControl.Overlays.Add(shipsOverlay);
        }

        //GMap control double click to Zoom and right Click to Zoom out
        private void GmapControl_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (gmapControl.Zoom < gmapControl.MaxZoom)
                {
                    gmapControl.Zoom++;
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (gmapControl.Zoom > gmapControl.MinZoom)
                {
                    gmapControl.Zoom--;
                }
            }

        }
        private void SetupMapContextMenu()
        {
            foreach (ToolStripMenuItem item in this.mapContextMenu.Items)
            {
                switch (item.Text)
                {
                    case "Zoom In":
                        item.Click += (s, e) => gmapControl.Zoom++;
                        break;
                    case "Zoom Out":
                        item.Click += (s, e) => gmapControl.Zoom--;
                        break;
                    case "Zoom Extents":
                        item.Click += (s, e) => gmapControl.ZoomAndCenterMarkers("ships");
                        break;
                    case "Copy Coordinates":
                        item.Click += (s, e) =>
                        {
                            var point = gmapControl.FromLocalToLatLng(
                                gmapControl.PointToClient(Cursor.Position).X,
                                gmapControl.PointToClient(Cursor.Position).Y);
                            Clipboard.SetText($"Lat: {point.Lat}, Lng: {point.Lng}");
                            //MessageBox.Show("Coordinates copied to clipboard!");
                        };
                        break;
                    case "Ship Track":
                    case "Route Calculator":
                    case "Add to My Ships":
                    case "Add Notification":
                    case "Add / Edit Note":
                        item.Click += (s, e) => MessageBox.Show($"{item.Text} feature coming soon!");
                        break;
                }
            }
        }

        private void LoadMockShips()
        {
            fleetManager.Ships = MockAISProvider.GetShips();
            shipsOverlay.Markers.Clear();
            shipsOverlay.Routes.Clear();

            foreach (var ship in fleetManager.Ships)
            {
                AddShipMarkerAndVector(ship);
            }
        }

        private List<Port> LoadPortsFromDatabase()
        {
            var ports = new List<Port>();
            using (var conn = new SqlConnection("Data Source=OMDESKTOP;Initial Catalog=ShipTracker;User ID=sk;Password=sk2005;"))
            {
                conn.Open();
                using (var cmd = new SqlCommand("SELECT PortId, PortName, Country, Latitude, Longitude, UNLOCODE FROM Ports", conn))
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

        private void ShowPortsOnMap()
        {
            var ports = LoadPortsFromDatabase();
            var portsOverlay = new GMapOverlay("ports");

            foreach (var port in ports)
            {
                var marker = new GMarkerGoogle(new PointLatLng(port.Latitude, port.Longitude), GMarkerGoogleType.green_small)
                {
                    ToolTipText = $"{port.PortName}"
                };
                portsOverlay.Markers.Add(marker);
            }
            gmapControl.Overlays.Add(portsOverlay);
        }

        

        private void AddShipMarkerAndVector(Ship ship)
        {
            GMarkerGoogleType markerType = GMarkerGoogleType.blue_dot;

            switch (ship.VesselType?.ToLower() ?? "")
            {
                case "cargo":
                    markerType = GMarkerGoogleType.blue_small;
                    break;
                case "tanker":
                    markerType = GMarkerGoogleType.red_small;
                    break;
                case "passenger":
                    markerType = GMarkerGoogleType.green_small;
                    break;
                case "fishing":
                    markerType = GMarkerGoogleType.yellow_small;
                    break;
                default:
                    markerType = GMarkerGoogleType.gray_small;
                    break;
            }

            var marker = new GMarkerGoogle(new PointLatLng(ship.Latitude, ship.Longitude), markerType)
            {
                ToolTipText = $"{ship.Name}\n{ship.VesselType}\nHeading: {ship.Heading}°",
                Tag = ship
            };

            shipsOverlay.Markers.Add(marker);

            // Draw heading vector
            double vectorLength = 0.05;  // ~5 km, adjust as needed
            double headingRad = Convert.ToDouble(ship.Heading) * Math.PI / 180;
            double destLat = ship.Latitude + (vectorLength * Math.Cos(headingRad));
            double destLng = ship.Longitude + (vectorLength * Math.Sin(headingRad)) / Math.Cos(ship.Latitude * Math.PI / 180);

            var vectorPoints = new List<PointLatLng>
            {
                new PointLatLng(ship.Latitude, ship.Longitude),
                new PointLatLng(destLat, destLng)
            };

            var vectorRoute = new GMapRoute(vectorPoints, $"{ship.Name}-Vector")
            {
                Stroke = new Pen(Color.Black, 1)
            };

            shipsOverlay.Routes.Add(vectorRoute);
        }

        private void Gmap_OnMarkerClick(GMapMarker item, MouseEventArgs e)
        {
            if (item.Tag is Ship ship)
            {
                MessageBox.Show(
                    $"Ship: {ship.Name}\n" +
                    $"IMO: {ship.IMO}\n" +
                    $"MMSI: {ship.MMSI}\n" +
                    $"Type: {ship.VesselType}\n" +
                    $"Status: {ship.Status}",
                    "Ship Details",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
        }
        private void SearchComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                var comboBox = sender as ToolStripComboBox;
                string query = comboBox.Text.Trim();
                if (string.IsNullOrWhiteSpace(query))
                    return;

                // Add to history if not duplicate
                if (!comboBox.Items.Contains(query))
                {
                    comboBox.Items.Insert(0, query);
                    if (comboBox.Items.Count > 10)
                        comboBox.Items.RemoveAt(10);
                }

                // Do the search...
                var ship = fleetManager.Ships.FirstOrDefault(s =>
                    (s.Name?.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (s.IMO?.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (s.MMSI?.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0));

                if (ship != null)
                {
                    gmapControl.Position = new PointLatLng(ship.Latitude, ship.Longitude);
                    gmapControl.Zoom = Math.Max(gmapControl.Zoom, 12);
                    MessageBox.Show($"Found ship:\nName: {ship.Name}\nIMO: {ship.IMO}\nMMSI: {ship.MMSI}", "Search Result");
                }
                else
                {
                    MessageBox.Show("No matching ship found.", "Search Result");
                }
                comboBox.Text = "";
                e.Handled = true;
                e.SuppressKeyPress = true;
                comboBox.Text = "Vessel Name / IMO / MMSI";
                comboBox.ForeColor = Color.Gray;

                if (string.IsNullOrWhiteSpace(searchComboBox.Text))
                {
                    searchComboBox.Text = "Vessel Name / IMO / MMSI";
                    searchComboBox.ForeColor = System.Drawing.Color.Gray;
                }                
            }
        }
        private void ZoomToExtent()
        {
            if (shipsOverlay.Markers.Count > 0)
            {
                var latitudes = shipsOverlay.Markers.Select(m => m.Position.Lat);
                var longitudes = shipsOverlay.Markers.Select(m => m.Position.Lng);
                var minLat = latitudes.Min();
                var maxLat = latitudes.Max();
                var minLng = longitudes.Min();
                var maxLng = longitudes.Max();
                var centerLat = (minLat + maxLat) / 2;
                var centerLng = (minLng + maxLng) / 2;
                gmapControl.Position = new PointLatLng(centerLat, centerLng);
                gmapControl.ZoomAndCenterMarkers("ships");
            }
        }
        private void RefreshMap()
        {
            shipsOverlay.Markers.Clear();
            shipsOverlay.Routes.Clear();
            LoadMockShips();  // Or reload from real source if connected
        }
        private void SearchComboBox_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripComboBox comboBox)
            {
                comboBox.Text = string.Empty;
            }
        }

        //MouseDown
        private void GmapControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                isMeasuringDistance = true;
                distanceStartPoint = gmapControl.FromLocalToLatLng(e.X, e.Y);
                gmapControl.ContextMenuStrip = null; // disable context menu temporarily
            }

        }
        //MouseMove
        private void GmapControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMeasuringDistance && distanceStartPoint.HasValue)
            {
                var currentPoint = gmapControl.FromLocalToLatLng(e.X, e.Y);
                double distanceNM = GetDistanceNM(distanceStartPoint.Value, currentPoint);
                double distanceKM = distanceNM * 1.852;
                double heading = GetBearing(distanceStartPoint.Value, currentPoint);

                var points = new List<PointLatLng> { distanceStartPoint.Value, currentPoint };

                if (distanceLine == null)
                {
                    distanceLine = new GMapRoute(points, "distanceLine")
                    {
                        Stroke = new Pen(Color.Black, 2)
                    };
                    shipsOverlay.Routes.Add(distanceLine);
                }
                else
                {
                    distanceLine.Points.Clear();
                    distanceLine.Points.AddRange(points);
                }

                if (distanceCircle == null)
                {
                    distanceCircle = new GMapCircle(distanceStartPoint.Value, (int)(distanceKM * 1000))
                    {
                        Stroke = new Pen(Color.Blue, 1)
                    };
                    shipsOverlay.Polygons.Add(distanceCircle);
                }
                else
                {
                    distanceCircle.Radius = distanceKM * 1000;
                    distanceCircle.Points.Clear();
                    distanceCircle.Points.AddRange(GMapCircle.CreateCirclePoints(distanceCircle.Position, distanceCircle.Radius));
                }
                gmapControl.Refresh();

                var point = gmapControl.FromLocalToLatLng(e.X, e.Y);
                latLabel.Text = $"Lat: {point.Lat:F4}";
                lonLabel.Text = $"Lon: {point.Lng:F4}";

            }
        }
        //MouseUp
        private void GmapControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (isMeasuringDistance && e.Button == MouseButtons.Right)
            {
                if (distanceLine != null) shipsOverlay.Routes.Remove(distanceLine);
                if (distanceCircle != null) shipsOverlay.Polygons.Remove(distanceCircle);

                distanceLine = null;
                distanceCircle = null;
                distanceStartPoint = null;
                isMeasuringDistance = false;

                // Restore context menu only AFTER right button is released
                gmapControl.ContextMenuStrip = mapContextMenu;

                // Manually show the context menu at the mouse position
                mapContextMenu.Show(gmapControl, e.Location);

                gmapControl.Refresh();
            }

        }

        private double GetDistanceNM(PointLatLng p1, PointLatLng p2)
        {
            double R = 6371000; // Earth radius in meters
            double lat1 = p1.Lat * Math.PI / 180;
            double lat2 = p2.Lat * Math.PI / 180;
            double dLat = (p2.Lat - p1.Lat) * Math.PI / 180;
            double dLon = (p2.Lng - p1.Lng) * Math.PI / 180;

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(lat1) * Math.Cos(lat2) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            double distanceMeters = R * c;
            double distanceNM = distanceMeters / 1852; // convert to nautical miles
            return distanceNM;
        }


        private double GetBearing(PointLatLng p1, PointLatLng p2)
        {
            var lat1 = Math.PI * p1.Lat / 180.0;
            var lat2 = Math.PI * p2.Lat / 180.0;
            var dLon = Math.PI * (p2.Lng - p1.Lng) / 180.0;

            var y = Math.Sin(dLon) * Math.Cos(lat2);
            var x = Math.Cos(lat1) * Math.Sin(lat2) -
                    Math.Sin(lat1) * Math.Cos(lat2) * Math.Cos(dLon);
            var brng = Math.Atan2(y, x);
            return (brng * 180.0 / Math.PI + 360) % 360;
        }

    }
}
