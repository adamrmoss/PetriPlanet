using System.Drawing;
using System.Windows.Forms;
using PetriPlanet.Core.Experiments;

namespace PetriPlanet
{
  public class ExperimentForm : Form
  {
    private ExperimentController controller;

    public ExperimentForm()
    {
      this.Initialize();
    }

    private void Initialize()
    {
      this.Text = "Petri Planet";
      this.BackColor = Color.DarkGray;

      var trackBar = new TrackBar {
        Orientation = Orientation.Horizontal,
        Minimum = 1,
        Maximum = 10,
        TickFrequency = 1, 
        TickStyle = TickStyle.BottomRight,
        SmallChange = 1,
        LargeChange = 10,
      };
      this.Controls.Add(trackBar);

      this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
      this.AutoSize = true;
    }

    public ExperimentForm SetController(ExperimentController experimentController)
    {
      this.controller = experimentController;
      return this;
    }

    public void Start()
    {
      var worldView = new WorldView(this.controller);
      this.Controls.Add(worldView);

      this.controller.Start();
    }
  }
}
