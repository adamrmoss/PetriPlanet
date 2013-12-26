using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetriPlanet.Core.Simulations
{
  public class SimulationController
  {
    public Simulation Simulation { get; private set; }

    public WorldGridElement[,] GetWorldGridElements()
    {
      var width = this.Simulation.WorldGrid.GetLength(0);
      var height = this.Simulation.WorldGrid.GetLength(1);
      var elements = new WorldGridElement[width, height];

      for (var i = 0; i < width; i++)
        for (var j = 0; j < height; j++)
          elements[i, j] = WorldGridElement.Build(this.Simulation.WorldGrid[i, j]);

      return elements;
    }

    private SimulationController()
    {
    }

    public static SimulationController Build(Simulation simulation)
    {
      return new SimulationController {
        Simulation = simulation,
      };
    }
  }
}
