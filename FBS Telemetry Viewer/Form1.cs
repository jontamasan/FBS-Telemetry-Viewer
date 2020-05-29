using Newtonsoft.Json;
using System;
using System.Drawing;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FBS_Telemetry_Viewer
{
    public partial class Form1 : Form
    {
        private static readonly HttpClient _client = new HttpClient();
        private static GetTelemetry telemetry;
        private DisplayData displayData;

        // URI to Ferunbus Telemetry
        const string BaseUri = "http://localhost:37337/";
        const string PlayerUri = BaseUri + "Player";
        const string Vehicles = BaseUri + "Vehicles";
        const string VehiclesCurrentUri = BaseUri + "Vehicles/Current";
        const string MissionUri = BaseUri + "Mission";
        const string MapUri = BaseUri + "Map";
        const string RouteUri = BaseUri + "Route";
        const string WorldUri = BaseUri + "World";
        const string RoadMapUri = BaseUri + "RoadMap";

        // DataGridView configure
        const int ColumnWidth = 152;
        const string HeaderNameText = "Name";
        const string HeaderValueText = "Value";
        DataGridView grid = new DataGridView();

        // ComboBox configure
        object[] intervals = new object[] { "100ms", "200ms", "300ms", "400ms", "500ms", "1000ms" };
        const int DefaultIntervalIndex = 4; // default: 500ms

        // Tab pages index
        enum Tab
        {
            Player,
            CurrentVehicle,
            Mission,
            Map,
            Route,
            World,
            RoadMap
        }

        public Form1()
        {
            InitializeComponent();

            telemetry = new GetTelemetry();
            displayData = new DisplayData();

            // Additional initialize components.
            //
            // button
            //
            button1.Text = "Connect";
            //
            // comboBox
            //
            comboBox1.Items.AddRange(intervals);
            comboBox1.SelectedIndex = DefaultIntervalIndex;
            //
            // grid view
            //
            dataGridViewSettings(dataGridView1); // Player
            dataGridViewSettings(dataGridView2); // Current Vehicle
            dataGridViewSettings(dataGridView3); // Mission
            dataGridViewSettings(dataGridView4); // Map
            dataGridViewSettings(dataGridView5); // Route
            dataGridViewSettings(dataGridView6); // World
            //dataGridViewSettings(dataGridView7); // Road Map
            //
            // timer event
            //
            timer1.Enabled = false;
            timer1.Interval = getNum_Inside_String((string)comboBox1.SelectedItem);
        }

        private void dataGridViewSettings(DataGridView dataGridView)
        {
            dataGridView.Dock = DockStyle.Fill;
            dataGridView.ReadOnly = true;
            dataGridView.RowHeadersVisible = false;
            dataGridView.AllowUserToAddRows = false;
            dataGridView.AllowUserToResizeRows = false;
            dataGridView.AllowUserToOrderColumns = false;
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;

            dataGridView.ColumnCount = 2;
            dataGridView.Columns[0].Width = ColumnWidth;
            dataGridView.Columns[0].HeaderText = HeaderNameText;
            dataGridView.Columns[1].Width = ColumnWidth;
            dataGridView.Columns[1].HeaderText = HeaderValueText;
        }

        // Set the interval of timer events when an item in comboBox is changed.
        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            timer1.Interval = getNum_Inside_String((string)comboBox1.SelectedItem);
        }

        private int getNum_Inside_String(string str)
        {
            return int.Parse(Regex.Replace(str, @"[^0-9]", ""));
        }

        // Connect button
        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;

            timer1.Enabled = !timer1.Enabled;
            if (timer1.Enabled)
            {
                label1.Text = "Connecting...";
                button1.Text = "Disconnect";
            }
            else
            {
                label1.Text = "Disconnected";
                button1.Text = "Connect";
            }

            button1.Enabled = true;
        }

        // デッドロックを回避するため、必ずasyncで実行
        private async void timer1_Tick(object sender, EventArgs e)
        {
            string errors = null;

            try
            {
                // Examine the open tab and connect only that tab.
                // Player
                if (this.tabControl1.SelectedIndex == (int)Tab.Player)
                {
                    // Allocate an instance of DataGridView for error display.
                    this.grid = this.dataGridView1;

                    // Call GetTelemetryAsync method then display data.
                    var playerResult = await telemetry.GetTelemetryAsync(PlayerUri);
                    if (playerResult != null)
                    {
                        displayData.Display(await playerResult.Content.ReadAsStringAsync(),
                            playerResult.Headers, this.dataGridView1, this.label1);
                    }
                }
                // Vehicle
                else if (this.tabControl1.SelectedIndex == (int)Tab.CurrentVehicle)
                {
                    this.grid = this.dataGridView2;
                    var currentVehicleResult = await telemetry.GetTelemetryAsync(VehiclesCurrentUri);
                    if (currentVehicleResult != null)
                    {
                        displayData.Display(await currentVehicleResult.Content.ReadAsStringAsync(),
                              currentVehicleResult.Headers, this.dataGridView2, this.label1);
                    }
                }
                // Mission
                else if (this.tabControl1.SelectedIndex == (int)Tab.Mission)
                {
                    this.grid = dataGridView3;
                    var missionResult = await telemetry.GetTelemetryAsync(MissionUri);
                    if (missionResult != null)
                    {
                        displayData.Display(await missionResult.Content.ReadAsStringAsync(),
                            missionResult.Headers, this.dataGridView3, this.label1);
                    }
                }
                // Map
                else if (this.tabControl1.SelectedIndex == (int)Tab.Map)
                {
                    this.grid = dataGridView4;
                    var mapResult = await telemetry.GetTelemetryAsync(MapUri);
                    if (mapResult != null)
                    {
                        displayData.Display(await mapResult.Content.ReadAsStringAsync(),
                            mapResult.Headers, this.dataGridView4, this.label1);
                    }
                }
                // Route (Navi)
                else if (this.tabControl1.SelectedIndex == (int)Tab.Route)
                {
                    this.grid = dataGridView5;
                    var routeResult = await telemetry.GetTelemetryAsync(RouteUri);
                    if (routeResult != null)
                    {
                        displayData.Display(await routeResult.Content.ReadAsStringAsync(),
                            routeResult.Headers, this.dataGridView5, this.label1);
                    }
                }
                // World
                else if (this.tabControl1.SelectedIndex == (int)Tab.World)
                {
                    this.grid = dataGridView6;
                    var worldResult = await telemetry.GetTelemetryAsync(WorldUri);
                    if (worldResult != null)
                    {
                        displayData.Display(await worldResult.Content.ReadAsStringAsync(),
                            worldResult.Headers, this.dataGridView6, this.label1);
                    }
                }
                // Road Map
                else if (this.tabControl1.SelectedIndex == (int)Tab.RoadMap)
                {
                    /* roadmapResult is not implementation, since this JSON data is quite
                     * huge that it is not updated in real time in the game.
                     * Code needs to be changed significantly when implementing. */
                }
            }
            catch (HttpRequestException httpex)
            {
                // 404 error, name resolution failure, etc.
                // Show exception messages recursively, including InnerException.
                Exception ex = httpex;
                while (ex != null)
                {
                    errors += $"{ex.Message}\n";
                    ex = ex.InnerException;
                }
            }
            catch (TaskCanceledException tcex)
            {
                // When tasks are canceled (e.g. timeout).
                errors = tcex.Message;
            }
            catch (JsonReaderException jex)
            {
                // Defect in json data etc.
                errors = jex.Message;
            }
            finally
            {
                if (errors != null)
                {
                    this.grid.RowCount = 0; // Grid clear.
                    this.grid.RowCount = 1; // Allocate grid for error display. 
                    this.grid[0, 0].Value = errors;
                }
            }
        }
    }
}
