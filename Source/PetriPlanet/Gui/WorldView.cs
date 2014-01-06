using System.Drawing;
using System.Windows.Forms;
using PetriPlanet.Core.Experiments;

namespace PetriPlanet.Gui
{
  public class WorldView : Control
  {
    private readonly Experiment experiment;

    public WorldView(Experiment experiment)
    {
      this.experiment = experiment;
      this.Initialize();
    }

    private void Initialize()
    {
      this.Width = (this.experiment.Width + 1) * WorldGridElement.WorldGridScale;
      this.Height = (this.experiment.Height + 1) * WorldGridElement.WorldGridScale;
      this.BackColor = Color.DarkGray;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      base.OnPaint(e);

      const int offset = WorldGridElement.WorldGridScale / 2;

      var worldGridElements = WorldGridElement.GetWorldGridElements(this.experiment.Organisms, this.experiment.Biomasses);
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
