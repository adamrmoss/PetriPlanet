using System;

namespace PetriPlanet.Core.Experiments
{
  public class ExperimentController
  {
    public Experiment Experiment { get; private set; }
    public WorldGridElement[,] GetWorldGridElements()
    {
      var width = this.Experiment.WorldGrid.GetLength(0);
      var height = this.Experiment.WorldGrid.GetLength(1);
      var elements = new WorldGridElement[width, height];

      for (var i = 0; i < width; i++)
        for (var j = 0; j < height; j++)
          elements[i, j] = WorldGridElement.Build(this.Experiment.WorldGrid[i, j]);

      return elements;
    }

    public event Action WorldGridUpdated;
    private void PublishWorldGridUpdated()
    {
      if (this.WorldGridUpdated != null)
        this.WorldGridUpdated();
    }

    private ExperimentController()
    {
    }

    public static ExperimentController Build(Experiment experiment)
    {
      return new ExperimentController {
        Experiment = experiment,
      };
    }

    public void Start()
    {
      var x = 5;
      var y = 10;
      var organism = new Organism {
        Energy = 248f,
      };

      this.Experiment.PlaceOrganism(organism, x, y);
    }
  }
}
