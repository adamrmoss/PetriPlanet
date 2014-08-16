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

    public int Width { get; private set; }
    public int Height { get; private set; }
    public int SunSize { get; private set; }

    public double EnergyDensity { get; private set; }
    public double EnergyBuffer { get; private set; }
    public double PhotosynthesisRate { get; set; }
    public int MinComplexity { get; set; }
    public int MaxComplexity { get; set; }
    public int MinPopulation { get; set; }

    public Organism[,] Organisms { get; private set; }
    public HashSet<Organism> SetOfOrganisms { get; private set; }

    public int Population
    {
      get { return this.SetOfOrganisms.Count; }
    }

    public int GetGenerations()
    {
      if (!this.SetOfOrganisms.Any())
        return 0;

      return this.SetOfOrganisms.Max(organism => organism.Generation);
    }

    public DateTime CurrentTime { get; private set; }
    public Random Random { get; private set; }

    public event Action<int, int> ExperimentUpdated;
    private void PublishExperimentUpdated(int x, int y)
    {
      if (this.ExperimentUpdated != null)
        this.ExperimentUpdated(x, y);
    }

    public Experiment(ExperimentSetup setup)
    {
      this.Random = new Random(setup.Seed);
      this.CurrentTime = setup.StartDate ?? dayOne;
      this.Width = setup.Width;
      this.Height = setup.Height;
      this.SunSize = setup.SunSize;
      this.EnergyDensity = setup.EnergyDensity;
      this.PhotosynthesisRate = setup.PhotosynthesisRate;
      this.MinComplexity = setup.MinComplexity;
      this.MaxComplexity = setup.MaxComplexity;
      this.MinPopulation = setup.MinPopulation;

      this.EnergyBuffer = (long) this.EnergyDensity * this.Width * this.Height;
      this.Organisms = new Organism[this.Width, this.Height];
      this.SetOfOrganisms = new HashSet<Organism>();
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
      this.PublishExperimentUpdated(organism.X, organism.Y);
    }

    public void DestroyOrganism(Organism organism)
    {
      if (!this.SetOfOrganisms.Contains(organism))
        throw new InvalidOperationException(String.Format("Organism not placed: {0}", organism));

      this.SetOfOrganisms.Remove(organism);
      this.Organisms[organism.X, organism.Y] = null;
      this.PublishExperimentUpdated(organism.X, organism.Y);
      this.CheckForExtinction();
    }

    public void Tick()
    {
      this.ProcessOrganisms();
      this.CurrentTime += tickIncrement;
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
      var energyCost = organism.Tick();
      organism.DeductEnergy(energyCost);
      this.EnergyBuffer += energyCost;

      if (organism.Health == 0) {
        this.DestroyOrganism(organism);
      }
    }

    private Tuple<ushort, ushort> FollowDirectionToPosition(int x, int y, Direction? direction)
    {
      if (direction == null)
        return null;

      var newX = (ushort) ((x + direction.Value.GetDeltaX() + this.Width) % this.Width);
      var newY = (ushort) ((y + direction.Value.GetDeltaY() + this.Height) % this.Height);
      return Tuple.Create(newX, newY);
    }

    public void Reproduce(Organism organism)
    {
      var facedPosition = this.FollowDirectionToPosition(organism.X, organism.Y, organism.Direction);
      var facedOrganism = this.Organisms[facedPosition.Item1, facedPosition.Item2];

      if (facedOrganism == null) {
        var energy = Math.Min(organism.Health, organism.Ax);
        organism.DeductEnergy(energy);

        var mutatedInstructions = organism.GetMutatedInstructions();
        var daughter = new Organism(Guid.NewGuid(), (ushort) (organism.Generation + 1), facedPosition.Item1, facedPosition.Item2, organism.Direction, energy, mutatedInstructions, this);
        this.SetupOrganism(daughter);
      }
    }

    public void Walk(Organism organism)
    {
      var facedPosition = this.FollowDirectionToPosition(organism.X, organism.Y, organism.Direction);
      var facedOrganism = this.Organisms[facedPosition.Item1, facedPosition.Item2];

      if (facedOrganism == null) {
        this.Organisms[organism.X, organism.Y] = null;
        organism.X = facedPosition.Item1;
        organism.Y = facedPosition.Item2;
        this.Organisms[organism.X, organism.Y] = organism;
      }
    }

    public ushort GetEnergyOfFacedOrganism(Organism organism)
    {
      var facedPosition = this.FollowDirectionToPosition(organism.X, organism.Y, organism.Direction);
      var facedOrganism = this.Organisms[facedPosition.Item1, facedPosition.Item2];

      return facedOrganism == null ? (ushort) 0 : facedOrganism.Health;
    }

    private void CheckForExtinction()
    {
      if (this.SetOfOrganisms.Count == 0)
        this.PublishExtinct();
    }
  }
}
