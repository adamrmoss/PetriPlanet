using System;
using System.Collections.Generic;
using System.Linq;
using PetriPlanet.Core.Collections;
using PetriPlanet.Core.Experiments;

namespace PetriPlanet.Core.Organisms
{
  public class Organism
  {
    private const int instructionCount = 1 << (8 * sizeof(ushort));
    private static Instruction[] allInstructions = EnumerableExtensions.GetAllEnumValues<Instruction>();

    private readonly Experiment experiment;

    public Instruction[] Instructions { get; private set; }
    public Direction FacingDirection { get; private set; }
    public ushort X { get; private set; }
    public ushort Y { get; private set; }
    public ushort Energy { get; private set; }

    public ushort Ax { get; private set; }
    public ushort Cx { get; private set; }
    public ushort Ip { get; private set; }

    public Organism(Experiment experiment, Instruction[] instructions, ushort x, ushort y, Direction facingDirection, ushort energy)
    {
      this.experiment = experiment;

      this.Instructions = BuildTotalInstructions(instructions);

      this.X = x;
      this.Y = y;
      this.FacingDirection = facingDirection;
      this.Energy = energy;
    }

    private Instruction[] BuildTotalInstructions(Instruction[] instructions)
    {
      if (instructions == null)
        return Enumerable.Range(0, instructionCount).Select(i => allInstructions.GetRandomElement(this.experiment.Random)).ToArray();

      if (instructions.Length > instructionCount)
        throw new InvalidOperationException("instructions.Length > instructionCount");

      if (instructions.Length == 0)
        return GetExtraInstructions(instructionCount).ToArray();

      var divisor = instructionCount / instructions.Length;
      var remainder = instructionCount % instructions.Length;

      var repeatedInstructions = instructions.Repeat(divisor);
      var extraInstructions = GetExtraInstructions(remainder);
      var totalInstructions = repeatedInstructions.Concat(extraInstructions).ToArray();
      return totalInstructions;
    }

    private static IEnumerable<Instruction> GetExtraInstructions(int extraInstructionCount)
    {
      return Enumerable.Repeat(Instruction.Nop, extraInstructionCount);
    }

    public void Tick()
    {
      var facingPosition = this.GetFacingPosition();
      var facedObject = this.experiment.WorldGrid[facingPosition.Item1, facingPosition.Item2];

      var currentInstruction = this.Instructions[this.Ip];
      switch (currentInstruction) {
        case Instruction.Nop:
          break;
        case Instruction.IfNot0:
          if (this.Cx == 0)
            this.IncrementIp();
          break;
        case Instruction.IfNotEq:
          if (this.Ax == this.Cx)
            this.IncrementIp();
          break;
        case Instruction.JumpBackward: {
          var previousNopIndex = this.PreviousNop(this.Ip);
          this.Ip = previousNopIndex;
          break;
        }
        case Instruction.JumpForward: {
          var nextNopIndex = this.NextNop(this.Ip);
          this.Ip = nextNopIndex;
          break;
        }
        case Instruction.ShiftLeft:
          this.Ax <<= 1;
          break;
        case Instruction.ShiftRight:
          this.Ax >>= 1;
          break;
        case Instruction.Increment:
          this.Cx++;
          break;
        case Instruction.Decrement:
          this.Cx--;
          break;
        case Instruction.Clear:
          this.Ax = 0;
          break;
        case Instruction.Add:
          this.Ax += this.Cx;
          break;
        case Instruction.Subtract:
          this.Ax -= this.Cx;
          break;
        case Instruction.Swap: {
          var temp = this.Ax;
          this.Ax = this.Cx;
          this.Cx = temp;
          break;
        }
        case Instruction.Sort:
          if (this.Ax > this.Cx)
            goto case Instruction.Swap;
          break;
        case Instruction.Reproduce:
          //throw new NotImplementedException();
          break;
        case Instruction.Excrete:
          //throw new NotImplementedException();
          break;
        case Instruction.Walk: {
          if (facedObject == null) {
            this.experiment.WorldGrid[this.X, this.Y] = null;
            this.X = facingPosition.Item1;
            this.Y = facingPosition.Item2;
            this.experiment.WorldGrid[this.X, this.Y] = this;
          }
          break;
        }
        case Instruction.TurnLeft:
          this.FacingDirection = this.FacingDirection.TurnLeft();
          break;
        case Instruction.TurnRight:
          this.FacingDirection = this.FacingDirection.TurnRight();
          break;
        case Instruction.Sense: {
          this.Ax = 0;
          this.Cx = 0;

          if (facedObject == null)
            break;

          var otherOrganism = facedObject as Organism;
          if (otherOrganism != null) {
            this.Ax = otherOrganism.Energy;
            break;
          }

          var biomass = facedObject as Biomass;
          if (biomass != null) {
            this.Cx = biomass.Value;
            break;
          }

          throw new NotImplementedException();
        }
        case Instruction.Imagine:
          this.Ax = (ushort) this.experiment.Random.Next(instructionCount);
          break;
      }

      this.IncrementIp();
    }

    private void IncrementIp()
    {
      this.Ip++;
    }

    private ushort PreviousNop(ushort startingIndex)
    {
      for (var index = startingIndex - 1; index > 0; index--)
        if (this.Instructions[index] == Instruction.Nop)
          return (ushort) index;
      for (var index = startingIndex - 1; index > startingIndex; index--)
        if (this.Instructions[index] == Instruction.Nop)
          return (ushort) index;
      return startingIndex;
    }

    private ushort NextNop(ushort startingIndex)
    {
      for (var index = startingIndex + 1; index < instructionCount; index++)
        if (this.Instructions[index] == Instruction.Nop)
          return (ushort) index;
      for (var index = 0; index < startingIndex; index++)
        if (this.Instructions[index] == Instruction.Nop)
          return (ushort) index;
      return startingIndex;
    }

    private Tuple<ushort, ushort> GetFacingPosition()
    {
      return FollowDirectionToPosition(this.X, this.Y, this.FacingDirection);
    }

    private Tuple<ushort, ushort> GetAvailableAdjacentPosition()
    {
      var potentialDirection = this.FacingDirection;
      for (var i = 0; i < 4; i++) {
        var potentialPosition = this.FollowDirectionToPosition(this.X, this.Y, potentialDirection);

        var neighbor = this.experiment.WorldGrid[potentialPosition.Item1, potentialPosition.Item2];
        if (neighbor == null)
          return potentialPosition;
      }

      return null;
    }

    private Tuple<ushort, ushort> FollowDirectionToPosition(ushort x, ushort y, Direction direction)
    {
      var width = this.experiment.WorldGrid.GetLength(0);
      var height = this.experiment.WorldGrid.GetLength(1);

      var newX = (ushort) ((x + direction.GetDeltaX() + width) % width);
      var newY = (ushort) ((y + direction.GetDeltaY() + height) % height);
      return Tuple.Create(newX, newY);
    }
  }
}
