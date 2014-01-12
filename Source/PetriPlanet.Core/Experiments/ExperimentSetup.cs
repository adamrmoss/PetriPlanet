using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PetriPlanet.Core.Organisms;

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
    public DateTime? StartDate { get; set; }
    public OrganismSetup[] Organisms { get; set; }
  }

  public class OrganismSetup
  {
    public ushort X { get; set; }
    public ushort Y { get; set; }
    public Direction Direction { get; set; }
    public Instruction[] Instructions { get; set; }
    public ushort Energy { get; set; }
  }
}
