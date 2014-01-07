using System;
using System.Collections.Generic;
using System.Linq;
using PetriPlanet.Core.Collections;
using PetriPlanet.Core.Experiments;

namespace PetriPlanet.Core.Organisms
{
  public class Organism
  {
    private static readonly Instruction[] allInstructions = EnumerableExtensions.GetAllEnumValues<Instruction>();

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
        return Enumerable.Range(0, Ushorts.Count).Select(i => allInstructions.GetRandomElement(this.experiment.Random)).ToArray();

      if (instructions.Length == 0)
        return GetExtraInstructions(Ushorts.Count).ToArray();

      if (instructions.Length > Ushorts.Count)
        throw new InvalidOperationException("instructions.Length > Ushorts.UshortCount");

      var divisor = Ushorts.Count / instructions.Length;
      var remainder = Ushorts.Count % instructions.Length;

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
      var presentBiomass = this.experiment.Biomasses[this.X, this.Y];
      if (presentBiomass != null) {
        if (presentBiomass.IsFood)
          this.AbsorbEnergy(presentBiomass.Value);
        else
          this.DeductEnergy(presentBiomass.Value);
        this.experiment.DestroyBiomass(presentBiomass);
      }

      var facedPosition = this.GetFacedPosition();
      var facedOrganism = this.experiment.Organisms[facedPosition.Item1, facedPosition.Item2];
      var facedBiomass = this.experiment.Biomasses[facedPosition.Item1, facedPosition.Item2];

      var currentInstruction = this.Instructions[this.Ip];

      var energyCost = EnergyCost(currentInstruction);
      this.DeductEnergy(energyCost);
      if (this.Energy == 0) {
        this.experiment.DestroyOrganism(this);
        return;
      }

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
          if (facedOrganism == null) {
            this.experiment.Organisms[this.X, this.Y] = null;
            this.X = facedPosition.Item1;
            this.Y = facedPosition.Item2;
            this.experiment.Organisms[this.X, this.Y] = this;
          }
          break;
        }
        case Instruction.TurnLeft:
          this.FacingDirection = this.FacingDirection.TurnLeft();
          break;
        case Instruction.TurnRight:
          this.FacingDirection = this.FacingDirection.TurnRight();
          break;
        case Instruction.Sense:
          this.Ax = facedOrganism != null ? facedOrganism.Energy : (ushort) 0;
          this.Cx = facedBiomass != null ? facedBiomass.Value : (ushort) 0;
          break;
        case Instruction.Imagine:
          this.Ax = (ushort) this.experiment.Random.Next(Ushorts.Count);
          break;
      }

      this.IncrementIp();
    }

    private void DeductEnergy(ushort energy)
    {
      this.Energy = (ushort) Math.Max(this.Energy - energy, 0);
    }

    private void AbsorbEnergy(ushort energy)
    {
      this.Energy = (ushort) Math.Min(this.Energy + energy, Ushorts.Count - 1);
    }

    private static ushort EnergyCost(Instruction instruction)
    {
      switch (instruction) {
        case Instruction.Nop:
        case Instruction.IfNot0:
        case Instruction.IfNotEq:
        case Instruction.JumpBackward:
        case Instruction.JumpForward:
        case Instruction.ShiftLeft:
        case Instruction.ShiftRight:
        case Instruction.Increment:
        case Instruction.Decrement:
        case Instruction.Clear:
        case Instruction.Add:
        case Instruction.Subtract:
        case Instruction.Swap:
        case Instruction.Sort:
          return 1;
        case Instruction.Reproduce:
          return 128;
        case Instruction.Excrete:
          return 4;
        case Instruction.Walk:
          return 8;
        case Instruction.TurnLeft:
        case Instruction.TurnRight:
          return 2;
        case Instruction.Sense:
          return 2;
        case Instruction.Imagine:
          return 2;
        default:
          throw new ArgumentException("Instruction not found: " + instruction);
      }
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
      for (var index = startingIndex + 1; index < Ushorts.Count; index++)
        if (this.Instructions[index] == Instruction.Nop)
          return (ushort) index;
      for (var index = 0; index < startingIndex; index++)
        if (this.Instructions[index] == Instruction.Nop)
          return (ushort) index;
      return startingIndex;
    }

    private Tuple<ushort, ushort> GetFacedPosition()
    {
      return FollowDirectionToPosition(this.X, this.Y, this.FacingDirection);
    }

    private Direction? GetDirectionToEmptyAdjacentPosition()
    {
      var directions = EnumerableExtensions.GetAllEnumValues<Direction>().Cast<Direction?>().ToArray();
      return directions.FirstOrDefault(direction => this.PositionIsEmpty(this.FollowDirectionToPosition(this.X, this.Y, direction)));
    }

    private Direction? GetDirectionToHabitableAdjacentPosition()
    {
      var directions = EnumerableExtensions.GetAllEnumValues<Direction>().Cast<Direction?>().ToArray();
      return directions.FirstOrDefault(direction => this.PositionIsEmpty(this.FollowDirectionToPosition(this.X, this.Y, direction))) ??
             directions.FirstOrDefault(direction => this.PositionIsUnoccupied(this.FollowDirectionToPosition(this.X, this.Y, direction)));
    }

    private Tuple<ushort, ushort> FollowDirectionToPosition(ushort x, ushort y, Direction? direction)
    {
      if (direction == null)
        return null;

      var newX = (ushort) ((x + direction.Value.GetDeltaX() + this.experiment.Width) % this.experiment.Width);
      var newY = (ushort) ((y + direction.Value.GetDeltaY() + this.experiment.Height) % this.experiment.Height);
      return Tuple.Create(newX, newY);
    }

    private bool PositionIsUnoccupied(Tuple<ushort, ushort> position)
    {
      return this.experiment.Organisms[position.Item1, position.Item2] == null;
    }

    private bool PositionIsEmpty(Tuple<ushort, ushort> position)
    {
      return this.experiment.Organisms[position.Item1, position.Item2] == null && this.experiment.Biomasses[position.Item1, position.Item2] == null;
    }

    }
  }
}
