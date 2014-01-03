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
    public int Width { get; set; }
    public int Height { get; set; }
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
    public int X { get; set; }
    public int Y { get; set; }
    public Instruction[] Instructions { get; set; }
    public ushort Value { get; set; }
    public float Energy { get; set; }
    public Direction Direction { get; set; }
  }
}
