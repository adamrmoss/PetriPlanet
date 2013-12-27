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
      this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
      this.AutoSize = true;
    }

    public ExperimentForm SetController(ExperimentController controller)
    {
      this.controller = controller;
      return this;
    }

    public void Start()
    {
      var experiment = this.controller.Experiment;
      this.Controls.Add(new WorldCanvas(experiment.Width, experiment.Height));
    }
  }
}
