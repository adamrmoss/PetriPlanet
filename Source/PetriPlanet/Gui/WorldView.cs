using System.Drawing;
using System.Windows.Forms;
using PetriPlanet.Core.Experiments;

namespace PetriPlanet.Gui
{
  public sealed class WorldView : Control
  {
    private const int offset = WorldGridElement.WorldGridScale / 2;
    private readonly Experiment experiment;
    private WorldGridElement[,] worldGridElements;

    public WorldView(Experiment experiment)
    {
      this.experiment = experiment;
      this.experiment.OnExperimentUpdated += this.OnExperimentUpdated;
      this.Width = (this.experiment.Width + 1) * WorldGridElement.WorldGridScale;
      this.Height = (this.experiment.Height + 1) * WorldGridElement.WorldGridScale;
      this.BackColor = Color.Black;

      this.worldGridElements = WorldGridElement.GetWorldGridElements(this.experiment.Organisms, this.experiment.Biomasses);
      this.Refresh();
    }

    protected override void OnPaint(PaintEventArgs eventArgs)
    {
      base.OnPaint(eventArgs);

      var minX = ToLogicalCoordinate(eventArgs.ClipRectangle.Left);
      var maxX = ToLogicalCoordinate(eventArgs.ClipRectangle.Right);

      var minY = ToLogicalCoordinate(eventArgs.ClipRectangle.Top);
      var maxY = ToLogicalCoordinate(eventArgs.ClipRectangle.Bottom);

      for (var x = minX; x < maxX; x++) {
        for (var y = minY; y < maxY; y++) {
          var worldGridElement = this.worldGridElements[x, y];

          var left = ToScreenCoordinate(x);
          var top = ToScreenCoordinate(y);
          worldGridElement.Draw(eventArgs.Graphics, left, top);
        }
      }
    }

    private static int ToScreenCoordinate(ushort x)
    {
      return x * WorldGridElement.WorldGridScale + offset;
    }

    private static ushort ToLogicalCoordinate(int x)
    {
      return (ushort) ((x - offset) / WorldGridElement.WorldGridScale);
    }

    private void OnExperimentUpdated(ushort x, ushort y)
    {
      this.worldGridElements[x, y] = WorldGridElement.GetWorldGridElement(this.experiment.Organisms[x, y], this.experiment.Biomasses[x, y]);
      this.Invalidate(new Rectangle(ToScreenCoordinate(x), ToScreenCoordinate(y), WorldGridElement.WorldGridScale, WorldGridElement.WorldGridScale));
    }
  }
}
