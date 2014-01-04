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
    public ExperimentSetupElement[] Elements { get; set; }
  }

  public enum ExperimentSetupElementType
  {
    Organism,
    Biomass,
  }

  public class ExperimentSetupElement
  {
    public ExperimentSetupElementType Type { get; set; }
    public Instruction[] Instructions { get; set; }
    public ushort X { get; set; }
    public ushort Y { get; set; }
    public Direction Direction { get; set; }
    public ushort Value { get; set; }
    public ushort Energy { get; set; }
  }
}
