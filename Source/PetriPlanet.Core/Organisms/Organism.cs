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
    public ushort[] Stack { get; private set; }

    public Direction FacingDirection { get; private set; }
    public ushort X { get; private set; }
    public ushort Y { get; private set; }
    public ushort Energy { get; private set; }

    public ushort Ax { get; private set; }
    public ushort Cx { get; private set; }
    public ushort Ip { get; private set; }
    public ushort Sp { get; private set; }

    public Organism(Experiment experiment, Instruction[] instructions, ushort x, ushort y, Direction facingDirection, ushort energy)
    {
      this.experiment = experiment;

      this.Instructions = BuildTotalInstructions(instructions);
      this.Stack = new ushort[Ushorts.Count];

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
        case Instruction.Push:
          this.Sp++;
          this.Stack[this.Sp] = this.Ax;
          break;
        case Instruction.Pop:
          this.Ax = this.Stack[this.Sp];
          this.Sp--;
          break;
        case Instruction.Peek:
          this.Ax = this.Stack[this.Sp];
          break;
        case Instruction.Reproduce:
          if (facedOrganism == null) {
            var energy = Math.Min(this.Energy, this.Ax);
            this.DeductEnergy(energy);

            var mutatedInstructions = this.GetMutatedInstructions();
            var daughter = new Organism(this.experiment, mutatedInstructions, facedPosition.Item1, facedPosition.Item2, FacingDirection, energy);
            this.experiment.SetupOrganism(daughter);
          }
          break;
        case Instruction.Excrete: {
          var currentValue = facedBiomass == null ? 0 : facedBiomass.Value;
          if (facedBiomass != null) {
            this.experiment.DestroyBiomass(facedBiomass);            
          }

          var excretedValue = Math.Min(this.Energy, this.Ax);
          this.DeductEnergy(excretedValue);

          var newValue = (ushort) (currentValue + excretedValue);
          if (newValue > 0) {
            var biomass = new Biomass(facedPosition.Item1, facedPosition.Item2, newValue);
          this.experiment.SetupBiomass(biomass);
          }

          break;
        }
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
          this.Ax = facedOrganism == null ? (ushort) 0 : facedOrganism.Energy;
          this.Cx = facedBiomass == null ? (ushort) 0 : facedBiomass.Value;
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
        case Instruction.Push:
        case Instruction.Pop:
        case Instruction.Peek:
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

    private Tuple<ushort, ushort> FollowDirectionToPosition(ushort x, ushort y, Direction? direction)
    {
      if (direction == null)
        return null;

      var newX = (ushort) ((x + direction.Value.GetDeltaX() + this.experiment.Width) % this.experiment.Width);
      var newY = (ushort) ((y + direction.Value.GetDeltaY() + this.experiment.Height) % this.experiment.Height);
      return Tuple.Create(newX, newY);
    }

    private Instruction[] GetMutatedInstructions()
    {
      var mutatedInstructions = new Instruction[Ushorts.Count];
      Array.Copy(this.Instructions, mutatedInstructions, Ushorts.Count);

      const int minSwaps = 100;
      const int maxSwaps = 1000;

      var numSwaps = this.experiment.Random.Next(minSwaps, maxSwaps);
      for (var i = 0; i < numSwaps; i++) {
        this.SwapRandomInstructionBlocks(mutatedInstructions);
      }

      const int minRandomizations = 100;
      const int maxRandomizations = 1000;
      var numRandomizations = this.experiment.Random.Next(minRandomizations, maxRandomizations);
      for (var i = 0; i < numRandomizations; i++) {
        var index = this.experiment.Random.Next(Ushorts.Count);
        mutatedInstructions[index] = allInstructions.GetRandomElement(this.experiment.Random);
      }

      return mutatedInstructions;
    }

    private void SwapRandomInstructionBlocks(Instruction[] instructions)
    {
      const ushort minLength = 5;
      const ushort maxLength = 128;

      var index = (ushort) this.experiment.Random.Next(Ushorts.Count);
      var firstBlockLength = (ushort) this.experiment.Random.Next(minLength, maxLength);
      var firstBlock = new Instruction[firstBlockLength];
      Arrays.WrapCopy(instructions, index, firstBlock, 0, firstBlockLength);

      var secondBlockLength = (ushort) this.experiment.Random.Next(minLength, maxLength);
      var secondBlock = new Instruction[secondBlockLength];
      Arrays.WrapCopy(instructions, (ushort) (index + firstBlockLength), secondBlock, 0, secondBlockLength);

      Arrays.WrapCopy(secondBlock, 0, instructions, index, secondBlockLength);
      Arrays.WrapCopy(firstBlock, 0, instructions, (ushort) (index + secondBlockLength), firstBlockLength);
    }
  }
}
