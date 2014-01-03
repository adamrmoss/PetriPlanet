using System;
using System.Collections.Generic;
using PetriPlanet.Core.Organisms;

namespace PetriPlanet.Core.Experiments
{
  public class Experiment
  {
    private static readonly TimeSpan tickIncrement = TimeSpan.FromSeconds(1);

    public int Width { get; private set; }
    public int Height { get; private set; }
    public object[,] WorldGrid { get; private set; }
    public HashSet<Organism> Organisms { get; private set; }
    public DateTime CurrentTime { get; private set; }

    public WorldGridElement[,] GetWorldGridElements()
    {
      var width = this.WorldGrid.GetLength(0);
      var height = this.WorldGrid.GetLength(1);
      var elements = new WorldGridElement[width, height];

      for (var i = 0; i < width; i++)
        for (var j = 0; j < height; j++)
          elements[i, j] = WorldGridElement.Build(this.WorldGrid[i, j]);

      return elements;
    }

    public event Action ExperimentUpdated;
    private void PublishExperimentUpdated()
    {
      if (this.ExperimentUpdated != null)
        this.ExperimentUpdated();
    }

    public Experiment(ExperimentSetup setup)
    {
      this.Width = setup.Width;
      this.Height = setup.Height;

      this.Organisms = new HashSet<Organism>();
      this.CurrentTime = new DateTime(1, 1, 1, 0, 0, 0);
      this.WorldGrid = new object[this.Width, this.Height];

      foreach (var experimentSetupElement in setup.Elements) {
        switch (experimentSetupElement.Type) {
          case ExperimentSetupElementType.Organism:
            var organism = new Organism(experimentSetupElement.Energy, experimentSetupElement.Direction, experimentSetupElement.Instructions);
            this.SetupOrganism(organism, experimentSetupElement.X, experimentSetupElement.Y);
            break;
          case ExperimentSetupElementType.Biomass:
            var biomass = new Biomass {
              Value = experimentSetupElement.Value,
            };
            this.SetupBiomass(biomass, experimentSetupElement.X, experimentSetupElement.Y);
            break;
        }
      }
    }

    public void SetupOrganism(Organism organism, int x, int y)
    {
      if (this.Organisms.Contains(organism))
        throw new InvalidOperationException(string.Format("Organism already placed: {0}", organism));

      var currentOccupant = this.WorldGrid[x, y];
      if (currentOccupant != null)
        throw new InvalidOperationException(string.Format("Position {0}, {1} already contains {2}", x, y, currentOccupant));

      this.WorldGrid[x, y] = organism;
      this.Organisms.Add(organism);
    }

    public void SetupBiomass(Biomass biomass, int x, int y)
    {
      this.WorldGrid[x, y] = biomass;
    }

    public void Tick()
    {
      this.CurrentTime += tickIncrement;
      this.PublishExperimentUpdated();
    }
  }
}
