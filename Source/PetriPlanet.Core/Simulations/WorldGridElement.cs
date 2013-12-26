using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetriPlanet.Core.Simulations
{
  public enum WorldGridElementType
  {
    Empty = 0,
    Organism = 1,
    Poison = 2,
    Food = 3,
  }

  public class WorldGridElement
  {
    private const float fullEnergyLevel = 100f;

    public WorldGridElementType Type { get; private set; }
    public float Intensity { get; private set; }

    public static WorldGridElement Build(object obj)
    {
      var organism = obj as Organism;

      if (obj == null) {
        return BuildEmpty();
      } else if (organism != null) {
        var intensity = organism.Energy / fullEnergyLevel;
        return BuildOrganism(intensity);
      } else
        throw new InvalidOperationException(string.Format("Unknown Object: {0}, cannot Build WorldGridElement", obj));
    }

    private static WorldGridElement BuildEmpty()
    {
      return new WorldGridElement { Type = WorldGridElementType.Empty };
    }

    private static WorldGridElement BuildOrganism(float intensity)
    {
      return new WorldGridElement { Type = WorldGridElementType.Organism, Intensity = intensity };
    }
  }
}
