using System;
using System.Drawing;
using System.Windows.Forms;
using PetriPlanet.Core.Experiments;
using PetriPlanet.Core.Maths;
using PetriPlanet.Core.Organisms;

namespace PetriPlanet.Gui
{
  public sealed class WorldView : Control
  {
    public const int WorldGridScale = 16;
    private const int offset = WorldGridScale / 2;

    private readonly Experiment experiment;

    public WorldView(Experiment experiment)
    {
      this.experiment = experiment;
      this.Width = (this.experiment.Width + 1) * WorldGridScale;
      this.Height = (this.experiment.Height + 1) * WorldGridScale;
      this.BackColor = Color.Black;

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
          var organism = this.experiment.Organisms[x, y];
          var left = ToScreenCoordinate(x);
          var top = ToScreenCoordinate(y);

          this.Draw(eventArgs.Graphics, organism, left, top);
        }
      }
    }

    private static int ToScreenCoordinate(int x)
    {
      return x * WorldGridScale + offset;
    }

    private static int ToLogicalCoordinate(int x)
    {
      return (x - offset) / WorldGridScale;
    }

    public Color GetOrganismColor(Organism organism)
    {
      var red = Math.Min((int) (organism.Red * organism.Health * 768), 256);
      var green = Math.Min((int) (organism.Green * organism.Health * 768), 256);
      var blue = Math.Min((int) (organism.Blue * organism.Health * 768), 256);
      return Color.FromArgb(red, green, blue);
    }

    public void Draw(Graphics graphics, Organism organism, int left, int top)
    {
      var backgroundBrush = new SolidBrush(Color.Black);
      graphics.FillRectangle(backgroundBrush, left, top, WorldGridScale, WorldGridScale);

      if (organism != null) {
        var organismBrush = new SolidBrush(this.GetOrganismColor(organism));
        var trianglePoints = GetTrianglePoints(organism.Direction, left, top);
        graphics.FillPolygon(organismBrush, trianglePoints);
      }
    }

    private static Point[] GetTrianglePoints(Direction direction, int left, int top)
    {
      switch (direction) {
        case Direction.East:
          return new[] {
            new Point(left + WorldGridScale / 2, top),
            new Point(left + WorldGridScale, top + WorldGridScale / 2),
            new Point(left + WorldGridScale / 2, top + WorldGridScale),
          };
        case Direction.North:
          return new[] {
            new Point(left, top + WorldGridScale / 2),
            new Point(left + WorldGridScale / 2, top),
            new Point(left + WorldGridScale, top + WorldGridScale / 2),
          };
        case Direction.West:
          return new[] {
            new Point(left + WorldGridScale / 2, top),
            new Point(left, top + WorldGridScale / 2),
            new Point(left + WorldGridScale / 2, top + WorldGridScale),
          };
        case Direction.South:
          return new[] {
            new Point(left, top + WorldGridScale / 2),
            new Point(left + WorldGridScale / 2, top + WorldGridScale),
            new Point(left + WorldGridScale, top + WorldGridScale / 2),
          };
        default:
          throw new ArgumentException("Direction not found: " + direction);
      }
    }

  }
}
