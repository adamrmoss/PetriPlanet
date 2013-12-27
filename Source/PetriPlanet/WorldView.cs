using System.Drawing;
using System.Windows.Forms;
using PetriPlanet.Core.Experiments;

namespace PetriPlanet
{
  public class WorldView : Control
  {
    private const int worldGridScale = 16;
    private ExperimentController experimentController;

    public WorldView(ExperimentController experimentController)
    {
      this.experimentController = experimentController;
      this.Initialize();
    }

    private void Initialize()
    {
      this.Width = (experimentController.Experiment.Width + 1) * worldGridScale;
      this.Height = (experimentController.Experiment.Height + 1) * worldGridScale;
      this.BackColor = Color.DarkGray;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      base.OnPaint(e);

      var brush = new SolidBrush(Color.LightGray);
      e.Graphics.FillRectangle(brush, worldGridScale / 2, worldGridScale / 2, experimentController.Experiment.Width * worldGridScale, experimentController.Experiment.Height * worldGridScale);
    }
  }
}
