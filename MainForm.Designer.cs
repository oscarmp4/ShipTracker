using System.Windows.Forms;
using GMap.NET.MapProviders;

namespace ShipTracker
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private GMap.NET.WindowsForms.GMapControl gmapControl;
        private System.Windows.Forms.MenuStrip mainMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem chartsMenu;
        private System.Windows.Forms.ToolStripMenuItem mapProviderMenu;
        private System.Windows.Forms.ToolStripMenuItem toolsMenu;
        private System.Windows.Forms.ToolStripMenuItem configureMenu;
        private System.Windows.Forms.ToolStripMenuItem helpMenu;
        private System.Windows.Forms.ToolStripComboBox searchComboBox;
        private System.Windows.Forms.ContextMenuStrip mapContextMenu;



        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.mainMenuStrip = new System.Windows.Forms.MenuStrip();
            this.chartsMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.mapProviderMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.configureMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.helpMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.searchComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.gmapControl = new GMap.NET.WindowsForms.GMapControl();
            this.mapContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.searchComboBox.Click += SearchComboBox_Click;

            // Charts dropdown
            this.chartsMenu.DropDownItems.Add(new ToolStripMenuItem("Traffic Analysis"));
            this.chartsMenu.DropDownItems.Add(new ToolStripMenuItem("Fleet Overview"));

            // Tools dropdown
            this.toolsMenu.DropDownItems.Add(new ToolStripMenuItem("Screenshot"));
            this.toolsMenu.DropDownItems.Add(new ToolStripMenuItem("Traffic Analysis"));
            this.toolsMenu.DropDownItems.Add(new ToolStripMenuItem("Notifications"));

            
            // Status bar
            var statusStrip = new StatusStrip();
            this.latLabel = new ToolStripStatusLabel() { Text = "Lat: 0.0000" };
            this.lonLabel = new ToolStripStatusLabel() { Text = "Lon: 0.0000" };
            var brightnessLabel = new ToolStripStatusLabel() { Text = "Brightness:" };
            var brightnessSlider = new ToolStripProgressBar() { Minimum = 0, Maximum = 100, Value = 100, Width = 100 };
            statusStrip.Items.Add(latLabel);
            statusStrip.Items.Add(lonLabel);
            //statusStrip.Items.Add(new ToolStripStatusLabel() { Spring = true }); // pushes right
            //statusStrip.Items.Add(brightnessLabel);
            //statusStrip.Items.Add(brightnessSlider);



            // Create a TrackBar (slider) for brightness
            this.brightnessTrackBar = new TrackBar()
            {
                Minimum = 0,
                Maximum = 100,
                Value = 100,
                TickStyle = TickStyle.None,
                Width = 100
            };

            // Wrap it in ToolStripControlHost to add into StatusStrip
            var trackBarHost = new ToolStripControlHost(brightnessTrackBar);

            statusStrip.Items.Add(new ToolStripStatusLabel("Brightness:"));
            statusStrip.Items.Add(trackBarHost);

            this.Controls.Add(statusStrip);


            // Set initial gray placeholder
            this.searchComboBox.Text = "Vessel Name / IMO / MMSI";
            this.searchComboBox.ForeColor = System.Drawing.Color.Gray;

            // Setup searchComboBox
            this.searchComboBox.Size = new System.Drawing.Size(250, 25);
            this.searchComboBox.Name = "searchComboBox";
            this.searchComboBox.ToolTipText = "Search by Name / IMO / MMSI";
            this.searchComboBox.KeyDown += SearchComboBox_KeyDown;

            // Setup zoom and refresh buttons
            var zoomInButton = new System.Windows.Forms.ToolStripButton()
            {
                DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image,
                Image = Properties.Resources.zoom_in_icon,
                ToolTipText = "Zoom In"
            };
            zoomInButton.Click += (s, e) => gmapControl.Zoom++;

            var zoomOutButton = new System.Windows.Forms.ToolStripButton()
            {
                DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image,
                Image = Properties.Resources.zoom_out_icon,
                ToolTipText = "Zoom Out"
            };
            zoomOutButton.Click += (s, e) => gmapControl.Zoom--;

            var zoomExtentButton = new System.Windows.Forms.ToolStripButton()
            {
                DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image,
                Image = Properties.Resources.zoom_extent_icon,
                ToolTipText = "Zoom Extents"
            };
            zoomExtentButton.Click += (s, e) => ZoomToExtent();

            var refreshButton = new System.Windows.Forms.ToolStripButton()
            {
                DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image,
                Image = Properties.Resources.refresh_icon,
                ToolTipText = "Refresh Map"
            };
            refreshButton.Click += (s, e) => RefreshMap();

            // Filler to push icons to right
            var filler = new System.Windows.Forms.ToolStripTextBox()
            {
                BorderStyle = BorderStyle.None,
                Size = new System.Drawing.Size(0, 0)
            };

            // Setup main menu
            this.mainMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.chartsMenu,
                this.mapProviderMenu,
                this.toolsMenu,
                this.configureMenu,
                this.helpMenu,
                filler,  //  pushes next items to the right
                this.searchComboBox,
                zoomInButton,
                zoomOutButton,
                zoomExtentButton,
                refreshButton
            });

            this.chartsMenu.Text = "Charts";
            this.mapProviderMenu.Text = "Map Provider";
            this.toolsMenu.Text = "Tools";
            this.configureMenu.Text = "Configure";
            this.helpMenu.Text = "Help";




            // Map provider options
            var googleMapItem = new ToolStripMenuItem("Google Maps");
            var bingMapItem = new ToolStripMenuItem("Bing Maps");
            var openSeaMapChartItem = new ToolStripMenuItem("OpenSeaMap Chart");
            var openStreetMapItem = new ToolStripMenuItem("OpenStreetMap");

            googleMapItem.Click += (s, e) => gmapControl.MapProvider = GoogleMapProvider.Instance;
            bingMapItem.Click += (s, e) => gmapControl.MapProvider = BingMapProvider.Instance;
            openSeaMapChartItem.Click += (s, e) => gmapControl.MapProvider = OpenSeaMapChartProvider.Instance;
            openStreetMapItem.Click += (s, e) => gmapControl.MapProvider = OpenStreetMapProvider.Instance;
            //DropDown Menu Options
            this.mapProviderMenu.DropDownItems.AddRange(new ToolStripItem[] { googleMapItem, bingMapItem, openSeaMapChartItem, openStreetMapItem, });


            //this.mapProviderMenu.DropDownItems.Add(openSeaMapItem);

            // Setup gmapControl
            this.gmapControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gmapControl.Bearing = 0F;
            this.gmapControl.CanDragMap = true;
            this.gmapControl.EmptyTileColor = System.Drawing.Color.Navy;
            this.gmapControl.GrayScaleMode = false;
            this.gmapControl.MarkersEnabled = true;
            this.gmapControl.MaxZoom = 20;
            this.gmapControl.MinZoom = 1;
            this.gmapControl.MouseWheelZoomEnabled = true;
            this.gmapControl.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionAndCenter;
            this.gmapControl.NegativeMode = false;
            this.gmapControl.PolygonsEnabled = true;
            this.gmapControl.RoutesEnabled = true;
            this.gmapControl.ShowTileGridLines = false;
            this.gmapControl.Zoom = 2D;
            this.gmapControl.MouseDoubleClick += GmapControl_MouseDoubleClick;

            // Context menu setup (right click map)
            this.mapContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                new System.Windows.Forms.ToolStripMenuItem("Zoom In"),
                new System.Windows.Forms.ToolStripMenuItem("Zoom Out"),
                new System.Windows.Forms.ToolStripMenuItem("Zoom Extents"),
                new System.Windows.Forms.ToolStripMenuItem("Copy Coordinates"),
                new System.Windows.Forms.ToolStripMenuItem("Ship Track"),
                new System.Windows.Forms.ToolStripMenuItem("Route Calculator"),
                new System.Windows.Forms.ToolStripMenuItem("Add to My Ships"),
                new System.Windows.Forms.ToolStripMenuItem("Add Notification"),
                new System.Windows.Forms.ToolStripMenuItem("Add / Edit Note")
            });

            this.gmapControl.ContextMenuStrip = this.mapContextMenu;

            // Add controls to form
            this.Controls.Add(this.gmapControl);
            this.Controls.Add(this.mainMenuStrip);
            this.MainMenuStrip = this.mainMenuStrip;
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.Text = "ShipTracker";
        }
    }
}
