using System;

namespace PetriPlanet.Core.Experiments
{
  public class ExperimentSetup
  {
    public int Seed { get; set; }
    public DateTime? StartDate { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public int SunSize { get; set; }
    public double EnergyDensity { get; set; }
    public double PhotosynthesisRate { get; set; }
    public int MinComplexity { get; set; }
    public int MaxComplexity { get; set; }
    public int MinPopulation { get; set; }
  }
}
