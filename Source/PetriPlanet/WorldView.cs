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

      var offset = worldGridScale / 2;

      var worldGridElements = experimentController.GetWorldGridElements();
      var width = worldGridElements.GetLength(0);
      var height = worldGridElements.GetLength(1);

      for (var i = 0; i < width; i++) {
        for (var j = 0; j < height; j++) {
          var left = offset + i * worldGridScale;
          var top = offset + j * worldGridScale;

          var brush = GetBrush(worldGridElements[i, j]);
          e.Graphics.FillRectangle(brush, left, top, worldGridScale, worldGridScale);
        }
      }
    }

    private Brush GetBrush(WorldGridElement worldGridElement)
    {
      return new SolidBrush(worldGridElement.GetColor());
    }
  }
}
