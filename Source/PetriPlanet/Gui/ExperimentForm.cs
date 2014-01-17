using System;
using System.Drawing;
using System.Windows.Forms;
using PetriPlanet.Core.Experiments;

namespace PetriPlanet.Gui
{
  public class ExperimentForm : Form
  {
    private const string descendingFormatString = "yyyy-MM-dd HH:mm:ss";
    private const string westernFormatString = "MM/dd/yyyy HH:mm:ss";
    private const int maxSpeed = 50;

    private readonly Experiment experiment;

    private FlowLayoutPanel verticalLayout;
    private TrackBar trackBar;
    private Label clockLabel;
    private Label environmentalPressureLabel;
    private Label populationLabel;
    private Label generationsLabel;
    private WorldView worldView;

    private Timer simulationTimer;
    private Timer uiTimer;

    public ExperimentForm(Experiment experiment)
    {
      this.experiment = experiment;
      this.experiment.Extinct += this.OnExtinct;
      this.Initialize();
    }

    private void Initialize()
    {
      this.MaximizeBox = false;
      this.Text = "Petri Planet";
      this.BackColor = Color.Black;

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

      this.environmentalPressureLabel = new Label {
        Text = string.Format("Environmental Pressure: {0}", this.experiment.EnvironmentalPressure),
        ForeColor = Color.LightGray,
        AutoSize = true,
        // Hackish
        Width = 200,
      };
      this.verticalLayout.Controls.Add(this.environmentalPressureLabel);

      this.populationLabel = new Label {
        Text = string.Format("Population: {0}", this.experiment.Population),
        ForeColor = Color.LightGray,
        AutoSize = true,
        // Hackish
        Width = 150,
      };
      this.verticalLayout.Controls.Add(this.populationLabel);

      this.generationsLabel = new Label {
        Text = string.Format("Generations: {0}", this.experiment.GetGenerations()),
        ForeColor = Color.LightGray,
        AutoSize = true,
        // Hackish
        Width = 150,
      };
      this.verticalLayout.Controls.Add(this.generationsLabel);

      this.worldView = new WorldView(this.experiment);
      this.verticalLayout.Controls.Add(this.worldView);

      var initialTime = this.experiment.CurrentTime.ToString(westernFormatString);
      this.clockLabel = new Label {
        Text = initialTime,
        ForeColor = Color.LightGray,
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
        Interval = 500,
      };
      this.simulationTimer.Tick += this.OnSimulationTick;

      this.uiTimer = new Timer {
        Enabled = true,
        Interval = 1000,
      };
      this.uiTimer.Tick += this.OnSlowUiTick;
    }

    private void OnSimulationTick(object sender, EventArgs eventArgs)
    {
      this.experiment.Tick();
      this.environmentalPressureLabel.Text = string.Format("Environmental Pressure: {0}", this.experiment.EnvironmentalPressure);
      this.populationLabel.Text = string.Format("Population: {0}", this.experiment.Population);
      this.generationsLabel.Text = string.Format("Generations: {0}", this.experiment.GetGenerations());
      this.clockLabel.Text = this.experiment.CurrentTime.ToString(westernFormatString);

      var trackBarValue = this.trackBar.Value;
      this.simulationTimer.Interval = trackBarValue == maxSpeed ? 1 : 500 / trackBarValue;
    }

    private void OnSlowUiTick(object sender, EventArgs eventArgs)
    {
      this.worldView.Refresh();
    }

    private void OnExtinct()
    {
      MessageBox.Show(this, "The population has gone extinct.", "Extinction");
      this.Close();
    }
  }
}
