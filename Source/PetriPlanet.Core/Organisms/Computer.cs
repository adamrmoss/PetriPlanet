using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetriPlanet.Core.Organisms
{
  public class Computer
  {
    private const int instructionCount = sizeof(short);
    public Instruction[] Instructions { get; private set; }

    public Direction FacingDirection { get; private set; }
    public ushort Bx { get; private set; }
    public ushort Cx { get; private set; }
    public ushort Ip { get; private set; }

    public Computer()
    {
      this.Instructions = Enumerable.Repeat(Instruction.NopX, instructionCount).ToArray();
    }
  }
}
