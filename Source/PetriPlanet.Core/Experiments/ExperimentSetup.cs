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
    public float EnvironmentalPressure { get; set; }
    public DateTime? StartDate { get; set; }
  }
}
