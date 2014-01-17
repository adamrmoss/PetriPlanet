using System;

namespace PetriPlanet.Core.Experiments
{
  public class ExperimentSetup
  {
    public int Seed { get; set; }
    public ushort Width { get; set; }
    public ushort Height { get; set; }
    public ushort EnergyDensity { get; set; }
    public ushort MinBiomassEnergy { get; set; }
    public ushort MaxBiomassEnergy { get; set; }
    public ushort BiomassRegenRate { get; set; }
    public ushort MinimumPopulation { get; set; }
    public ushort MaximumPopulation { get; set; }
    public float MinimumEnvironmentalPressure { get; set; }
    public float MaximumEnvironmentalPressure { get; set; }
    public DateTime? StartDate { get; set; }
  }
}
