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

    protected ushort EnergyDensity { get; private set; }
    public long EnergyBuffer { get; private set; }
    public ushort MinBiomassEnergy { get; private set; }
    public ushort MaxBiomassEnergy { get; private set; }

    public Biomass[,] Biomasses { get; private set; }
    public Organism[,] Organisms { get; private set; }
    public HashSet<Organism> SetOfOrganisms { get; private set; }

    public DateTime CurrentTime { get; private set; }
    public Random Random { get; private set; }

    public Experiment(ExperimentSetup setup)
    {
      this.Width = setup.Width;
      this.Height = setup.Height;
      this.EnergyDensity = setup.EnergyDensity;
      this.MinBiomassEnergy = setup.MinBiomassEnergy;
      this.MaxBiomassEnergy = setup.MaxBiomassEnergy;

      this.Random = new Random(setup.Seed);
      this.CurrentTime = setup.StartDate ?? dayOne;

      this.EnergyBuffer = (long) this.EnergyDensity * this.Width * this.Height;
      this.Biomasses = new Biomass[this.Width, this.Height];
      this.Organisms = new Organism[this.Width, this.Height];
      this.SetOfOrganisms = new HashSet<Organism>();

      foreach (var organismSetup in setup.Organisms) {
        var organism = new Organism(this, organismSetup.Instructions, organismSetup.X, organismSetup.Y, organismSetup.Direction, organismSetup.Energy);
        this.SetupOrganism(organism);
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
      if (this.EnergyBuffer > this.MaxBiomassEnergy) {
        var position = this.FindRandomAvailableBiomassPosition();
        if (position != null) {
          var minEnergy = (ushort) Math.Min(this.EnergyBuffer, this.MinBiomassEnergy);
          var maxEnergy = (ushort) Math.Min(this.EnergyBuffer, this.MaxBiomassEnergy);
          var energy = (ushort) this.Random.Next(minEnergy, maxEnergy);
          var newBiomass = new Biomass(position.Item1, position.Item2, energy);
          this.EnergyBuffer -= energy;

          this.SetupBiomass(newBiomass);
        }
      }

      foreach (var organism in this.SetOfOrganisms.ToArray()) {
        organism.Tick();
      }
      this.CurrentTime += tickIncrement;
    }

    private Tuple<ushort, ushort> FindRandomAvailableBiomassPosition()
    {
      const int retryCount = 64;
      for (var i = 0; i < retryCount; i++) {
        var x = this.Random.Next(this.Width);
        var y = this.Random.Next(this.Height);

        if (this.Biomasses[x, y] == null)
          return Tuple.Create((ushort) x, (ushort) y);
      }
      return null;
    }
  }
}
