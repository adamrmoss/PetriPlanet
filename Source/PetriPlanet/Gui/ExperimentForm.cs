using System;
using System.Drawing;
using System.Windows.Forms;
using PetriPlanet.Core.Experiments;

namespace PetriPlanet.Gui
{
  public class ExperimentForm : Form
  {
    //private const string dreamtimeFormatString = "yyyy-MM-dd HH:mm:ss";
    private const string dreamtimeFormatString = "MM/dd/yyyy HH:mm:ss";
    private const int maxSpeed = 100;

    private readonly Experiment experiment;

    private FlowLayoutPanel verticalLayout;
    private TrackBar trackBar;
    private Label clockLabel;
    private WorldView worldView;

    private Timer simulationTimer;
    private Timer uiTimer;

    public ExperimentForm(Experiment experiment)
    {
      this.experiment = experiment;
      this.Initialize();
    }

    private void Initialize()
    {
      this.Text = "Petri Planet";
      this.BackColor = Color.LightGray;

      this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
      this.AutoSize = true;

      this.verticalLayout = new FlowLayoutPanel {
        FlowDirection = FlowDirection.TopDown,
        AutoSizeMode = AutoSizeMode.GrowAndShrink,
        AutoSize = true,
      };
      this.Controls.Add(this.verticalLayout);

      var headerLayout = new FlowLayoutPanel {
        FlowDirection = FlowDirection.LeftToRight,
        AutoSizeMode = AutoSizeMode.GrowAndShrink,
        AutoSize = true,
      };
      this.verticalLayout.Controls.Add(headerLayout);

      this.worldView = new WorldView(this.experiment);
      this.verticalLayout.Controls.Add(this.worldView);

      var initialTime = this.experiment.CurrentTime.ToString(dreamtimeFormatString);
      this.clockLabel = new Label {
        Text = initialTime,
        AutoSize = true,
        // Hackish
        Width = 150,
      };
      headerLayout.Controls.Add(this.clockLabel);

      this.trackBar = new TrackBar {
        Orientation = Orientation.Horizontal,
        Minimum = 1,
        Maximum = maxSpeed,
        TickFrequency = 5,
        TickStyle = TickStyle.BottomRight,
        SmallChange = 1,
        LargeChange = 10,
        AutoSize = true,
        Width = this.worldView.Width - this.clockLabel.Width
      };
      headerLayout.Controls.Add(this.trackBar);

      this.simulationTimer = new Timer {
        Enabled = true,
        Interval = 1000,
      };
      this.simulationTimer.Tick += this.OnSimulationTick;

      this.uiTimer = new Timer {
        Enabled = true,
        Interval = 100,
      };
      this.uiTimer.Tick += this.OnUiTick;
    }

    private void OnSimulationTick(object sender, EventArgs eventArgs)
    {
      this.experiment.Tick();
      this.clockLabel.Text = this.experiment.CurrentTime.ToString(dreamtimeFormatString);
      var trackBarValue = this.trackBar.Value;
      this.simulationTimer.Interval = trackBarValue == maxSpeed ? 1 : 1000 / trackBarValue;
    }

    private void OnUiTick(object sender, EventArgs eventArgs)
    {
      this.clockLabel.Refresh();
      this.worldView.Refresh();
    }
  }
}
