using System;
using System.Drawing;
using System.Windows.Forms;
using PetriPlanet.Core.Experiments;

namespace PetriPlanet
{
  public class ExperimentForm : Form
  {
    private ExperimentController controller;

    private FlowLayoutPanel verticalLayout;
    private TrackBar trackBar;
    private Label clockLabel;

    public ExperimentForm()
    {
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

      var initialTime = this.controller == null ? "-" : this.controller.Experiment.CurrentTime.ToString();
      this.clockLabel = new Label {
        Text = initialTime,
      };
      headerLayout.Controls.Add(this.clockLabel);

      this.trackBar = new TrackBar {
        Orientation = Orientation.Horizontal,
        Minimum = 1,
        Maximum = 10,
        TickFrequency = 1,
        TickStyle = TickStyle.BottomRight,
        SmallChange = 1,
        LargeChange = 10,
      };
      headerLayout.Controls.Add(this.trackBar);
    }

    public ExperimentForm SetController(ExperimentController experimentController)
    {
      this.controller = experimentController;
      return this;
    }

    public void Start()
    {
      var worldView = new WorldView(this.controller);

      this.verticalLayout.Controls.Add(worldView);

      this.controller.Start();
    }
  }
}
