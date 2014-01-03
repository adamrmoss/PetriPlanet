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

    public Computer(Direction facingDirection, Instruction[] instructions)
    {
      this.FacingDirection = facingDirection;

      if (instructions.Length > instructionCount)
        throw new InvalidOperationException("instructions.Length > instructionCount");

      var extraInstructionCount = instructionCount - instructions.Length;
      var extraInstructions = Enumerable.Repeat(Instruction.NopX, extraInstructionCount);
      var totalInstructions = instructions.Concat(extraInstructions).ToArray();
      this.Instructions = totalInstructions;
    }
  }
}
