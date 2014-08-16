using System;
using System.Drawing;
using PetriPlanet.Core;
using PetriPlanet.Core.Organisms;

namespace PetriPlanet.Gui
{
  public class WorldGridElement
  {
    public const int WorldGridScale = 16;

    public double Intensity { get; private set; }
    public Direction Direction { get; private set; }

    public Organism Organism { get; set; }

    public Color GetColor()
    {
      var red = 256 * Organism.A
      return this.Type.GetColor().ApplyIntensity(this.Intensity);
    }

    public void Draw(Graphics graphics, int left, int top)
    {
      var brush = new SolidBrush(this.GetColor());

      switch (this.Type) {
        case WorldGridElementType.Empty:
          graphics.FillRectangle(brush, left, top, WorldGridScale, WorldGridScale);
          break;
        case WorldGridElementType.Organism:
          var trianglePoints = GetTrianglePoints(this.Direction, left, top);
          graphics.FillPolygon(brush, trianglePoints);
          break;
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

    public static WorldGridElement BuildOrganismElement(Organism organism)
    {
      var intensity = ((2 * organism.Health / Ushorts.Count) + .5) / 3;
      return new WorldGridElement {
        Intensity = intensity,
        Direction = organism.Direction,
      };
    }

    public static WorldGridElement BuildEmpty()
    {
      return new WorldGridElement {
        Intensity = 1.0,
      };
    }

    public static WorldGridElement[,] GetWorldGridElements(Organism[,] organisms)
    {
      var width = organisms.GetLength(0);
      var height = organisms.GetLength(1);
      var elements = new WorldGridElement[width, height];

      for (var x = 0; x < width; x++) {
        for (var y = 0; y < height; y++) {
          var organism = organisms[x, y];
          var worldGridElement = GetWorldGridElement(organism);
          elements[x, y] = worldGridElement;
        }
      }

      return elements;
    }

    public static WorldGridElement GetWorldGridElement(Organism organism)
    {
      var worldGridElement = organism != null ? BuildOrganismElement(organism) : BuildEmpty();
      return worldGridElement;
    }
  }
}
