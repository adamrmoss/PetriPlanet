using System;
using System.Collections.Generic;
using PetriPlanet.Core.Organisms;

namespace PetriPlanet.Core.Experiments
{
  public class Experiment
  {
    private static readonly TimeSpan tickIncrement = TimeSpan.FromSeconds(1);
    private static readonly DateTime dayOne = new DateTime(1, 1, 1, 0, 0, 0);

    public ushort Width { get; private set; }
    public ushort Height { get; private set; }
    public object[,] WorldGrid { get; private set; }
    public HashSet<Organism> Organisms { get; private set; }
    public DateTime CurrentTime { get; private set; }
    public Random Random { get; private set; }

    public Experiment(ExperimentSetup setup)
    {
      this.Width = setup.Width;
      this.Height = setup.Height;
      this.Random = new Random(setup.Seed);

      this.Organisms = new HashSet<Organism>();
      this.CurrentTime = dayOne;
      this.WorldGrid = new object[this.Width, this.Height];

      foreach (var experimentSetupElement in setup.Elements) {
        switch (experimentSetupElement.Type) {
          case ExperimentSetupElementType.Organism:
            var organism = new Organism(this, experimentSetupElement.Instructions, experimentSetupElement.X, experimentSetupElement.Y, experimentSetupElement.Direction, experimentSetupElement.Energy);
            this.SetupOrganism(organism);
            break;
          case ExperimentSetupElementType.Biomass:
            var biomass = new Biomass(experimentSetupElement.X, experimentSetupElement.Y,experimentSetupElement.Value);
            this.SetupBiomass(biomass);
            break;
        }
      }
    }

    public void SetupOrganism(Organism organism)
    {
      if (this.Organisms.Contains(organism))
        throw new InvalidOperationException(string.Format("Organism already placed: {0}", organism));

      var currentOccupant = this.WorldGrid[organism.X, organism.Y];
      if (currentOccupant != null)
        throw new InvalidOperationException(string.Format("Position {0}, {1} already contains {2}", organism.X, organism.Y, currentOccupant));

      this.WorldGrid[organism.X, organism.Y] = organism;
      this.Organisms.Add(organism);
    }

    public void SetupBiomass(Biomass biomass)
    {
      this.WorldGrid[biomass.X, biomass.Y] = biomass;
    }

    public void Tick()
    {
      foreach (var organism in this.Organisms) {
        organism.Tick();
      }
      this.CurrentTime += tickIncrement;
    }
    }
  }
}
