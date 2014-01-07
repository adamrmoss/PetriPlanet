using System;
using System.Drawing;
using PetriPlanet.Core;
using PetriPlanet.Core.Organisms;

namespace PetriPlanet.Gui
{
  public enum WorldGridElementType
  {
    Empty = 0,
    Organism = 1,
    Poison = 2,
    Food = 3,
  }

  public static class WorldGridElementTypeExtensions
  {
    public static Color GetColor(this WorldGridElementType worldGridElementType)
    {
      switch (worldGridElementType) {
        case WorldGridElementType.Empty:
          return Color.DarkGray;
        case WorldGridElementType.Organism:
          return Color.LightGreen;
        case WorldGridElementType.Poison:
          return Color.Firebrick;
        case WorldGridElementType.Food:
          return Color.CornflowerBlue;
        default:
          throw new ArgumentException(string.Format("Cannot handle worldGridElementType: {0}", worldGridElementType));
      }
    }
  }

  public class WorldGridElement
  {
    public const int WorldGridScale = 16;

    public WorldGridElementType Type { get; private set; }
    public float Intensity { get; private set; }
    public Direction Direction { get; private set; }

    public Color GetColor()
    {
      return this.Type.GetColor().ApplyIntensity(this.Intensity);
    }

    public void Draw(Graphics graphics, int left, int top)
    {
      var brush = new SolidBrush(this.GetColor());

      switch (this.Type) {
        case WorldGridElementType.Empty:
        case WorldGridElementType.Food:
        case WorldGridElementType.Poison:
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
      var intensity = (float) organism.Energy / Ushorts.Count;
      return new WorldGridElement {
        Type = WorldGridElementType.Organism,
        Intensity = intensity,
        Direction = organism.FacingDirection,
      };
    }

    public static WorldGridElement BuildBiomassElement(Biomass biomass)
    {
      var type = Primes.LookupTable[biomass.Value] ? WorldGridElementType.Food : WorldGridElementType.Poison;
      var intensity = (float) biomass.Value / Ushorts.Count;

      return new WorldGridElement {
        Type = type,
        Intensity = intensity,
      };
    }

    public static WorldGridElement BuildEmpty()
    {
      return new WorldGridElement {
        Type = WorldGridElementType.Empty,
        Intensity = 1f,
      };
    }

    public static WorldGridElement[,] GetWorldGridElements(Organism[,] organisms, Biomass[,] biomasses)
    {
      var width = organisms.GetLength(0);
      var height = organisms.GetLength(1);
      var elements = new WorldGridElement[width, height];

      for (var i = 0; i < width; i++) {
        for (var j = 0; j < height; j++) {
          var organism = organisms[i, j];
          var biomass = biomasses[i, j];
          elements[i, j] = organism != null ? BuildOrganismElement(organism) :
                           biomass != null ? BuildBiomassElement(biomass) : BuildEmpty();
        }
      }

      return elements;
    }
  }
}
