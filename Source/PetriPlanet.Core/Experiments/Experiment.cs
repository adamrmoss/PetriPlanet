﻿using System;
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
    protected ushort BiomassRegenRate { get; set; }
    protected float PoisonEfficacy { get; set; }

    public Biomass[,] Biomasses { get; private set; }
    public Organism[,] Organisms { get; private set; }
    public HashSet<Organism> SetOfOrganisms { get; private set; }

    public DateTime CurrentTime { get; private set; }
    public Random Random { get; private set; }

    public event Action<ushort, ushort> OnExperimentUpdated;
    private void PublishOnExperimentUpdated(ushort x, ushort y)
    {
      if (this.OnExperimentUpdated != null)
        this.OnExperimentUpdated(x, y);
    }

    public Experiment(ExperimentSetup setup)
    {
      this.Width = setup.Width;
      this.Height = setup.Height;
      this.EnergyDensity = setup.EnergyDensity;
      this.MinBiomassEnergy = setup.MinBiomassEnergy;
      this.MaxBiomassEnergy = setup.MaxBiomassEnergy;
      this.BiomassRegenRate = setup.BiomassRegenRate;
      this.PoisonEfficacy = setup.PoisonEfficacy;

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
        throw new InvalidOperationException(String.Format("Organism already placed: {0}", organism));

      var currentOccupant = this.Organisms[organism.X, organism.Y];
      if (currentOccupant != null)
        throw new InvalidOperationException(String.Format("Position {0}, {1} already contains {2}", organism.X, organism.Y, currentOccupant));

      this.Organisms[organism.X, organism.Y] = organism;
      this.SetOfOrganisms.Add(organism);
      this.PublishOnExperimentUpdated(organism.X, organism.Y);
    }

    public void DestroyOrganism(Organism organism)
    {
      if (!this.SetOfOrganisms.Contains(organism))
        throw new InvalidOperationException(String.Format("Organism not placed: {0}", organism));

      this.SetOfOrganisms.Remove(organism);
      this.Organisms[organism.X, organism.Y] = null;
      this.PublishOnExperimentUpdated(organism.X, organism.Y);
    }

    public void SetupBiomass(Biomass biomass)
    {
      this.Biomasses[biomass.X, biomass.Y] = biomass;
      this.PublishOnExperimentUpdated(biomass.X, biomass.Y);
    }

    public void DestroyBiomass(Biomass biomass)
    {
      this.Biomasses[biomass.X, biomass.Y] = null;
      this.PublishOnExperimentUpdated(biomass.X, biomass.Y);
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
        this.PublishOnExperimentUpdated(oldX, oldY);
        this.PublishOnExperimentUpdated(organism.X, organism.Y);
      }
    }

    private void ProcessOrganism(Organism organism)
    {
      var presentBiomass = this.Biomasses[organism.X, organism.Y];
      if (presentBiomass != null) {
        if (presentBiomass.IsFood) {
          organism.AbsorbEnergy(presentBiomass.Energy);
        } else {
          var energyToDeduct = (ushort) (presentBiomass.Energy * this.PoisonEfficacy);
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
        var daughter = new Organism(this, mutatedInstructions, facedPosition.Item1, facedPosition.Item2, organism.FacingDirection, energy);
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
  }
}
