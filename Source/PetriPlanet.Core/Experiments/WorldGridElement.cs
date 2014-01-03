using System;
using System.Drawing;
using PetriPlanet.Core.Organisms;

namespace PetriPlanet.Core.Experiments
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
          return Color.Green;
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
    private const float fullEnergyLevel = 256f;

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
          var trianglePoints = GetTrianglePoints(Direction, left, top);
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
          throw new ArgumentException("Direction cannot be null: " + direction);
      }
    }

    public static WorldGridElement Build(object obj)
    {
      var organism = obj as Organism;
      var biomass = obj as Biomass;

      if (obj == null) {
        return BuildEmpty();
      } else if (organism != null) {
        var intensity = organism.Energy / fullEnergyLevel;
        return BuildOrganism(intensity, organism.FacingDirection);
      } else if (biomass != null) {
        return BuildBiomass(biomass.Value);
      } else
        throw new InvalidOperationException(String.Format("Unknown Object: {0}, cannot Build WorldGridElement", obj));
    }

    private static WorldGridElement BuildEmpty()
    {
      return new WorldGridElement {
        Type = WorldGridElementType.Empty, 
        Intensity = 1f,
      };
    }

    private static WorldGridElement BuildOrganism(float intensity, Direction direction)
    {
      return new WorldGridElement {
        Type = WorldGridElementType.Organism, 
        Intensity = intensity, 
        Direction = direction,
      };
    }

    private static WorldGridElement BuildBiomass(ushort value)
    {
      var type = Primes.LookupTable[value] ? WorldGridElementType.Food : WorldGridElementType.Poison;

      return new WorldGridElement {
        Type = type,
        Intensity = 1f,
      };
    }

    public const int WorldGridScale = 16;
  }
}
