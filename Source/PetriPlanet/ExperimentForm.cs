using System;
using System.Drawing;
using System.Windows.Forms;
using PetriPlanet.Core.Experiments;

namespace PetriPlanet
{
  public class ExperimentForm : Form
  {
    private const string dreamtimeFormatString = "yyyy-MM-dd HH:mm:ss";

    private readonly ExperimentController controller;

    private FlowLayoutPanel verticalLayout;
    private TrackBar trackBar;
    private Label clockLabel;
    private WorldView worldView;

    private Timer simulationTimer;
    private Timer uiTimer;

    public ExperimentForm(ExperimentController controller)
    {
      this.controller = controller;
      this.Initialize();
    }

    private void Initialize()
    {
      this.Text = "Petri Planet";
      this.BackColor = Color.DarkGray;

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

      this.worldView = new WorldView(this.controller);
      this.verticalLayout.Controls.Add(this.worldView);

      var initialTime = this.controller.Experiment.CurrentTime.ToString(dreamtimeFormatString);
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
        Maximum = 100,
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
      this.controller.Tick();
      this.clockLabel.Text = this.controller.Experiment.CurrentTime.ToString(dreamtimeFormatString);
      this.simulationTimer.Interval = 1000 / this.trackBar.Value;
    }

    private void OnUiTick(object sender, EventArgs eventArgs)
    {
      this.clockLabel.Refresh();
      this.worldView.Refresh();
    }
  }
}
