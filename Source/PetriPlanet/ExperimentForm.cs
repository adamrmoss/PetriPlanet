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
      this.Controls.Add(new WorldView(this.controller));
    }
  }
}
