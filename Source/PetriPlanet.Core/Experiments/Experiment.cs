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

    public ushort EnergyDensity { get; private set; }
    public long EnergyBuffer { get; private set; }
    public ushort MinBiomassEnergy { get; private set; }
    public ushort MaxBiomassEnergy { get; private set; }
    public ushort BiomassRegenRate { get; set; }

    public ushort MinimumPopulation { get; set; }
    public ushort MaximumPopulation { get; set; }
    public float MinimumEnvironmentalPressure { get; set; }
    public float MaximumEnvironmentalPressure { get; set; }
    public float EnvironmentalPressure { get; set; }

    public Biomass[,] Biomasses { get; private set; }
    public Organism[,] Organisms { get; private set; }
    public HashSet<Organism> SetOfOrganisms { get; private set; }

    public int Population
    {
      get { return this.SetOfOrganisms.Count; }
    }

    public ushort GetGenerations()
    {
      if (!this.SetOfOrganisms.Any())
        return 0;

      return this.SetOfOrganisms.Max(organism => organism.Generation);
    }

    public DateTime CurrentTime { get; private set; }
    public Random Random { get; private set; }

    public event Action<ushort, ushort> ExperimentUpdated;
    private void PublishExperimentUpdated(ushort x, ushort y)
    {
      if (this.ExperimentUpdated != null)
        this.ExperimentUpdated(x, y);
    }

    public event Action Extinct;
    private void PublishExtinct()
    {
      if (this.Extinct != null)
        this.Extinct();
    }

    public Experiment(ExperimentSetup setup)
    {
      this.Width = setup.Width;
      this.Height = setup.Height;
      this.EnergyDensity = setup.EnergyDensity;
      this.MinBiomassEnergy = setup.MinBiomassEnergy;
      this.MaxBiomassEnergy = setup.MaxBiomassEnergy;
      this.BiomassRegenRate = setup.BiomassRegenRate;

      this.MinimumPopulation = setup.MinimumPopulation;
      this.MaximumPopulation = setup.MaximumPopulation;
      this.MinimumEnvironmentalPressure = setup.MinimumEnvironmentalPressure;
      this.MaximumEnvironmentalPressure = setup.MaximumEnvironmentalPressure;

      this.Random = new Random(setup.Seed);
      this.CurrentTime = setup.StartDate ?? dayOne;

      this.EnergyBuffer = (long) this.EnergyDensity * this.Width * this.Height;
      this.Biomasses = new Biomass[this.Width, this.Height];
      this.Organisms = new Organism[this.Width, this.Height];
      this.SetOfOrganisms = new HashSet<Organism>();

      this.EnvironmentalPressure = this.ComputeEnvironmentalPressure();
    }

    private float ComputeEnvironmentalPressure()
    {
      if (this.Population < this.MinimumPopulation)
        return this.MinimumEnvironmentalPressure;

      if (this.Population > this.MaximumPopulation)
        return this.MaximumEnvironmentalPressure;

      var populationRange = this.MaximumPopulation - this.MinimumPopulation;
      var pressureRange = this.MaximumEnvironmentalPressure - this.MinimumEnvironmentalPressure;

      var populationRatio = ((float) this.Population - this.MinimumPopulation) / populationRange;
      var environmentalPressure = (populationRatio * pressureRange) + this.MinimumEnvironmentalPressure;
      return environmentalPressure;
    }

    public void SetupOrganisms(IEnumerable<Organism> organisms)
    {
      foreach (var organism in organisms) {
        this.SetupOrganism(organism);
      }
    }

    public void SetupOrganism(Organism organism)
    {
      if (this.SetOfOrganisms.Contains(organism))
        throw new InvalidOperationException(String.Format("Organism already placed: {0}", organism));

      var currentOccupant = this.Organisms[organism.X, organism.Y];
      if (currentOccupant != null)
        throw new InvalidOperationException(String.Format("Position {0}, {1} already contains {2}", organism.X, organism.Y, currentOccupant));

      this.Organisms[organism.X, organism.Y] = organism;
      this.SetOfOrganisms.Add(organism);
      this.EnvironmentalPressure = this.ComputeEnvironmentalPressure();
      this.PublishExperimentUpdated(organism.X, organism.Y);
    }

    public void DestroyOrganism(Organism organism)
    {
      if (!this.SetOfOrganisms.Contains(organism))
        throw new InvalidOperationException(String.Format("Organism not placed: {0}", organism));

      this.SetOfOrganisms.Remove(organism);
      this.Organisms[organism.X, organism.Y] = null;
      this.EnvironmentalPressure = this.ComputeEnvironmentalPressure();
      this.PublishExperimentUpdated(organism.X, organism.Y);
      this.CheckForExtinction();
    }

    public void SetupBiomass(Biomass biomass)
    {
      this.Biomasses[biomass.X, biomass.Y] = biomass;
      this.PublishExperimentUpdated(biomass.X, biomass.Y);
    }

    public void DestroyBiomass(Biomass biomass)
    {
      this.Biomasses[biomass.X, biomass.Y] = null;
      this.PublishExperimentUpdated(biomass.X, biomass.Y);
    }

    public void Tick()
    {
      for (var i = 0; i < this.BiomassRegenRate; i++) {
        this.RegenerateBiomass();
      }
      this.ProcessOrganisms();
      this.CurrentTime += tickIncrement;
    }

    private void RegenerateBiomass()
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
    }

    private void ProcessOrganisms()
    {
      foreach (var organism in this.SetOfOrganisms.ToArray()) {
        var oldX = organism.X;
        var oldY = organism.Y;
        this.ProcessOrganism(organism);
        this.PublishExperimentUpdated(oldX, oldY);
        this.PublishExperimentUpdated(organism.X, organism.Y);
      }
    }

    private void ProcessOrganism(Organism organism)
    {
      var presentBiomass = this.Biomasses[organism.X, organism.Y];
      if (presentBiomass != null) {
        if (presentBiomass.IsFood) {
          organism.AbsorbEnergy(presentBiomass.Energy);
        } else {
          var energyToDeduct = (ushort) (presentBiomass.Energy * this.EnvironmentalPressure);
          organism.DeductEnergy(energyToDeduct);
          this.EnergyBuffer += presentBiomass.Energy * 2;
        }
        this.DestroyBiomass(presentBiomass);
      }

      var energyCost = organism.Tick();
      organism.DeductEnergy(energyCost);
      this.EnergyBuffer += energyCost;

      if (organism.Energy == 0) {
        this.DestroyOrganism(organism);
      }
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

    private Tuple<ushort, ushort> FollowDirectionToPosition(ushort x, ushort y, Direction? direction)
    {
      if (direction == null)
        return null;

      var newX = (ushort) ((x + direction.Value.GetDeltaX() + this.Width) % this.Width);
      var newY = (ushort) ((y + direction.Value.GetDeltaY() + this.Height) % this.Height);
      return Tuple.Create(newX, newY);
    }

    public void Excrete(Organism organism, ushort excretedValue)
    {
      var facedPosition = this.FollowDirectionToPosition(organism.X, organism.Y, organism.FacingDirection);
      var facedBiomass = this.Biomasses[facedPosition.Item1, facedPosition.Item2];

      var currentValue = facedBiomass == null ? 0 : facedBiomass.Energy;
      if (facedBiomass != null) {
        this.DestroyBiomass(facedBiomass);
      }

      organism.DeductEnergy(excretedValue);

      var newValue = (ushort) (currentValue + excretedValue);
      if (newValue > 0) {
        var biomass = new Biomass(facedPosition.Item1, facedPosition.Item2, newValue);
        this.SetupBiomass(biomass);
      }
    }

    public void Reproduce(Organism organism)
    {
      var facedPosition = this.FollowDirectionToPosition(organism.X, organism.Y, organism.FacingDirection);
      var facedOrganism = this.Organisms[facedPosition.Item1, facedPosition.Item2];

      if (facedOrganism == null) {
        var energy = Math.Min(organism.Energy, organism.Ax);
        organism.DeductEnergy(energy);

        var mutatedInstructions = organism.GetMutatedInstructions();
        var daughter = new Organism(Guid.NewGuid(), (ushort) (organism.Generation + 1), facedPosition.Item1, facedPosition.Item2, organism.FacingDirection, energy, mutatedInstructions, this);
        this.SetupOrganism(daughter);
      }
    }

    public void Walk(Organism organism)
    {
      var facedPosition = this.FollowDirectionToPosition(organism.X, organism.Y, organism.FacingDirection);
      var facedOrganism = this.Organisms[facedPosition.Item1, facedPosition.Item2];

      if (facedOrganism == null) {
        this.Organisms[organism.X, organism.Y] = null;
        organism.X = facedPosition.Item1;
        organism.Y = facedPosition.Item2;
        this.Organisms[organism.X, organism.Y] = organism;
      }
    }

    public ushort GetEnergyOfFacedBiomass(Organism organism)
    {
      var facedPosition = this.FollowDirectionToPosition(organism.X, organism.Y, organism.FacingDirection);
      var facedBiomass = this.Biomasses[facedPosition.Item1, facedPosition.Item2];

      return facedBiomass == null ? (ushort) 0 : facedBiomass.Energy;
    }

    public ushort GetEnergyOfFacedOrganism(Organism organism)
    {
      var facedPosition = this.FollowDirectionToPosition(organism.X, organism.Y, organism.FacingDirection);
      var facedOrganism = this.Organisms[facedPosition.Item1, facedPosition.Item2];

      return facedOrganism == null ? (ushort) 0 : facedOrganism.Energy;
    }

    private void CheckForExtinction()
    {
      if (this.SetOfOrganisms.Count == 0)
        this.PublishExtinct();
    }
  }
}
