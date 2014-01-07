using System;
using System.Collections.Generic;
using System.Linq;
using PetriPlanet.Core.Organisms;

namespace PetriPlanet.Core.Experiments
{
  public class Experiment
  {
    private static readonly TimeSpan tickIncrement = TimeSpan.FromSeconds(1);
    private static readonly DateTime dayOne = new DateTime(1, 1, 1, 0, 0, 0);

    public ushort Width { get; private set; }
    public ushort Height { get; private set; }
    public Biomass[,] Biomasses { get; private set; }
    public Organism[,] Organisms { get; private set; }
    public HashSet<Organism> SetOfOrganisms { get; private set; }

    public DateTime CurrentTime { get; private set; }
    public Random Random { get; private set; }

    public Experiment(ExperimentSetup setup)
    {
      this.Width = setup.Width;
      this.Height = setup.Height;
      this.Random = new Random(setup.Seed);

      this.CurrentTime = dayOne;
      this.Biomasses = new Biomass[this.Width,this.Height];
      this.Organisms = new Organism[this.Width, this.Height];
      this.SetOfOrganisms = new HashSet<Organism>();

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
      if (this.SetOfOrganisms.Contains(organism))
        throw new InvalidOperationException(string.Format("Organism already placed: {0}", organism));

      var currentOccupant = this.Organisms[organism.X, organism.Y];
      if (currentOccupant != null)
        throw new InvalidOperationException(string.Format("Position {0}, {1} already contains {2}", organism.X, organism.Y, currentOccupant));

      this.Organisms[organism.X, organism.Y] = organism;
      this.SetOfOrganisms.Add(organism);
    }

    public void DestroyOrganism(Organism organism)
    {
      if (!this.SetOfOrganisms.Contains(organism))
        throw new InvalidOperationException(string.Format("Organism not placed: {0}", organism));
      this.SetOfOrganisms.Remove(organism);

      this.Organisms[organism.X, organism.Y] = null;
      var value = (ushort) (this.Random.NextDouble() * this.Random.Next(Ushorts.Count));
      var corpse = new Biomass(organism.X, organism.Y, value);
      this.SetupBiomass(corpse);
    }

    public void SetupBiomass(Biomass biomass)
    {
      this.Biomasses[biomass.X, biomass.Y] = biomass;
    }

    public void DestroyBiomass(Biomass biomass)
    {
      this.Biomasses[biomass.X, biomass.Y] = null;
    }

    public void Tick()
    {
      foreach (var organism in this.SetOfOrganisms.ToArray()) {
        organism.Tick();
      }
      this.CurrentTime += tickIncrement;
    }
  }
}
