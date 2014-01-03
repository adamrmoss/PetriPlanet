using System.Drawing;
using System.Windows.Forms;
using PetriPlanet.Core.Experiments;

namespace PetriPlanet
{
  public class WorldView : Control
  {
    private readonly ExperimentController experimentController;

    public WorldView(ExperimentController experimentController)
    {
      this.experimentController = experimentController;
      this.Initialize();
    }

    private void Initialize()
    {
      this.Width = (experimentController.Experiment.Width + 1) * WorldGridElement.WorldGridScale;
      this.Height = (experimentController.Experiment.Height + 1) * WorldGridElement.WorldGridScale;
      this.BackColor = Color.DarkGray;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      base.OnPaint(e);

      const int offset = WorldGridElement.WorldGridScale / 2;

      var worldGridElements = experimentController.GetWorldGridElements();
      var width = worldGridElements.GetLength(0);
      var height = worldGridElements.GetLength(1);

      for (var i = 0; i < width; i++) {
        for (var j = 0; j < height; j++) {
          var left = offset + i * WorldGridElement.WorldGridScale;
          var top = offset + j * WorldGridElement.WorldGridScale;

          var worldGridElement = worldGridElements[i, j];
          worldGridElement.Draw(e.Graphics, left, top);
        }
      }
    }
  }
}
