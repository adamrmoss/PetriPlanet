using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
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
      var json = File.ReadAllText(@"ExperimentSetup.json");
      var experimentSetup = JsonConvert.DeserializeObject<ExperimentSetupElement[]>(json);
      foreach (var experimentSetupElement in experimentSetup) {
        switch (experimentSetupElement.Type) {
          case ExperimentSetupElementType.Organism:
            var computer = new Computer(experimentSetupElement.Direction, experimentSetupElement.Instructions);

            var organism = new Organism(computer) {
              Energy = experimentSetupElement.Energy,
            };
            this.Experiment.PlaceOrganism(organism, experimentSetupElement.X, experimentSetupElement.Y);
            break;
          case ExperimentSetupElementType.Biomass:
            var biomass = new Biomass {
              Value = experimentSetupElement.Value,
            };
            this.Experiment.PlaceBiomass(biomass, experimentSetupElement.X, experimentSetupElement.Y);
            break;
        }
      }
    }

    public void Tick()
    {
      this.Experiment.Tick();
      this.PublishExperimentUpdated();
    }
  }
}
