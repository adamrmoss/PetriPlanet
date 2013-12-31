using System;
using System.Collections.Generic;
using PetriPlanet.Core.Organisms;

namespace PetriPlanet.Core.Experiments
{
  public class Experiment
  {
    public int Width { get; private set; }
    public int Height { get; private set; }
    public object[,] WorldGrid { get; private set; }
    public HashSet<Organism> Organisms { get; private set; }
    
    public DateTime CurrentTime { get; private set; }
    public int TimeScale { get; private set; }

    private DateTime lastCheckedTime;

    private Experiment()
    {
      this.Organisms = new HashSet<Organism>();

      this.CurrentTime = new DateTime();
      this.TimeScale = 1;
      this.lastCheckedTime = DateTime.Now;
    }

    public static Experiment Build(int width, int height)
    {
      return new Experiment {
        Width = width,
        Height = height,
        WorldGrid = new object[width, height],
      };
    }

    public void PlaceOrganism(Organism organism, int x, int y)
    {
      if (this.Organisms.Contains(organism))
        throw new InvalidOperationException(string.Format("Organism already placed: {0}", organism));

      var currentOccupant = this.WorldGrid[x, y];
      if (currentOccupant != null)
        throw new InvalidOperationException(string.Format("Position {0}, {1} already contains {2}", x, y, currentOccupant));

      this.WorldGrid[x, y] = organism;
      this.Organisms.Add(organism);
    }

    public void PlaceBiomass(Biomass biomass, int x, int y)
    {
      this.WorldGrid[x, y] = biomass;
    }

    public void UpdateCurrentTime()
    {
      var rawTimeSpan = DateTime.Now - this.lastCheckedTime;
      var scaledTicks = this.TimeScale * rawTimeSpan.Ticks;
      var scaledTimeSpan = new TimeSpan(scaledTicks);

      this.CurrentTime += scaledTimeSpan;
      this.lastCheckedTime = DateTime.Now;
    }
  }
}
