using System;
using System.Linq;
using PetriPlanet.Core.Experiments;

namespace PetriPlanet.Core.Organisms
{
  public class Organism
  {
    private const int instructionCount = sizeof(short);

    private readonly Experiment experiment;

    public Instruction[] Instructions { get; private set; }
    public Direction FacingDirection { get; private set; }

    public float Energy { get; private set; }
    public ushort Bx { get; private set; }
    public ushort Cx { get; private set; }
    public ushort Ip { get; private set; }

    public Organism(Experiment experiment, Instruction[] instructions, Direction facingDirection, float energy)
    {
      this.experiment = experiment;
      this.Energy = energy;
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
