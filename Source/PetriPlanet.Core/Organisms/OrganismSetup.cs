using System;

namespace PetriPlanet.Core.Organisms
{
  public class OrganismSetup
  {
    public Guid Id { get; set; }
    public int Generation { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public Direction Direction { get; set; }

    public double Red { get; set; }
    public double Green { get; set; }
    public double Blue { get; set; }

    public double Health { get; set; }
    public double Steering { get; set; }
    public double Motor { get; set; }
    public double Aggression { get; set; }
    public double Reproduction { get; set; }

    public Instruction[] SteeringInstructions { get; set; }
    public Instruction[] MotorInstructions { get; set; }
    public Instruction[] AggressionInstructions { get; set; }
    public Instruction[] ReproductionInstructions { get; set; }
  }
}
