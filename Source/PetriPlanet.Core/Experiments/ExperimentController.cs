using System;
using PetriPlanet.Core.Organisms;

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

    public event Action ExperimentUpdated;
    private void PublishExperimentUpdated()
    {
      if (this.ExperimentUpdated != null)
        this.ExperimentUpdated();
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
      var organism = new Organism {
        Energy = 248f,
        Direction = Direction.South,
      };

      this.Experiment.PlaceOrganism(organism, 10, 8);

      var biomass1 = new Biomass {
        Value = 64000,
      };
      this.Experiment.PlaceBiomass(biomass1, 11, 8);

      var biomass2 = new Biomass {
        Value = 65521,
      };
      this.Experiment.PlaceBiomass(biomass2, 9, 8);

      var biomass3 = new Biomass {
        Value = 29,
      };
      this.Experiment.PlaceBiomass(biomass3, 9, 9);
    }

    public void Tick()
    {
      this.Experiment.Tick();
      this.PublishExperimentUpdated();
    }
  }
}
