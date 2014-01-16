using System;

namespace PetriPlanet.Core.Organisms
{
  public class OrganismSetup
  {
    public Guid Id { get; set; }
    public ushort X { get; set; }
    public ushort Y { get; set; }
    public Direction Direction { get; set; }
    public Instruction[] Instructions { get; set; }
    public ushort Energy { get; set; }
  }
}