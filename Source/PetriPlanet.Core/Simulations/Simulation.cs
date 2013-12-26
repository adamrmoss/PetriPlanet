using System;
using System.Collections.Generic;

namespace PetriPlanet.Core.Simulations
{
  public class Simulation
  {
    public object[,] WorldGrid { get; private set; }
    public HashSet<Organism> Organisms { get; private set; }

    private Simulation()
    {
    }

    public static Simulation Build(int width, int height)
    {
      return new Simulation {
        WorldGrid = new object[width, height]
      };
    }

    public void PlaceOrganism(Organism organism, int x, int y)
    {
      if (Organisms.Contains(organism))
        throw new InvalidOperationException(string.Format("Organism already placed: {0}", organism));

      var currentOccupant = WorldGrid[x, y];
      if (currentOccupant != null)
        throw new InvalidOperationException(string.Format("Position {0}, {1} already contains {2}", x, y, currentOccupant));

      this.WorldGrid[x, y] = organism;
    }
  }
}
