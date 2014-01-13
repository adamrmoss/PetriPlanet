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
    public ushort X { get; set; }
    public ushort Y { get; set; }
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
      return Enumerable.Repeat(Instruction.Label, extraInstructionCount);
    }

    public Instruction[] GetMutatedInstructions()
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

    public ushort Tick()
    {
      var currentInstruction = this.Instructions[this.Ip];

      var energyCost = currentInstruction.GetEnergyCost();

      switch (currentInstruction) {
        case Instruction.Label:
        case Instruction.Skip:
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
          var count = 1;
          while (this.Instructions[this.Ip + 1] == Instruction.Skip) {
            count++;
            this.IncrementIp();
          }

          var previousNopIndex = this.PreviousLabel(this.Ip, count);
          this.Ip = previousNopIndex;
          break;
        }
        case Instruction.JumpForward: {
            var count = 1;
            while (this.Instructions[this.Ip + 1] == Instruction.Skip) {
              count++;
              this.IncrementIp();
            }

            var nextNopIndex = this.NextLabel(this.Ip, count);
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
          this.experiment.Reproduce(this);
          break;
        case Instruction.Excrete: {
          var excretedValue = Math.Min(this.Energy, this.Ax);
          this.experiment.Excrete(this, excretedValue);
          break;
        }
        case Instruction.Walk:
          this.experiment.Walk(this);
          break;
        case Instruction.TurnLeft:
          this.FacingDirection = this.FacingDirection.TurnLeft();
          break;
        case Instruction.TurnRight:
          this.FacingDirection = this.FacingDirection.TurnRight();
          break;
        case Instruction.Sense:
          this.Ax = this.experiment.GetEnergyOfFacedOrganism(this);
          this.Cx = this.experiment.GetEnergyOfFacedBiomass(this);
          break;
        case Instruction.Imagine:
          this.Ax = (ushort) this.experiment.Random.Next(Ushorts.Count);
          break;
      }

      this.IncrementIp();
      return energyCost;
    }

    public void DeductEnergy(ushort energy)
    {
      this.Energy = (ushort) Math.Max(this.Energy - energy, 0);
    }

    public void AbsorbEnergy(ushort energy)
    {
      this.Energy = (ushort) Math.Min(this.Energy + energy, Ushorts.Count - 1);
    }

    private void IncrementIp()
    {
      this.Ip++;
    }

    private ushort PreviousLabel(ushort startingIndex, int iterations)
    {
      if (iterations == 0)
        return startingIndex;

      for (var index = startingIndex - 1; index > 0; index--)
        if (this.Instructions[index] == Instruction.Label)
          return this.PreviousLabel((ushort) index, iterations - 1);
      for (var index = startingIndex - 1; index > startingIndex; index--)
        if (this.Instructions[index] == Instruction.Label)
          return this.PreviousLabel((ushort) index, iterations - 1);

      return startingIndex;
    }

    private ushort NextLabel(ushort startingIndex, int iterations)
    {
      if (iterations == 0)
        return startingIndex;

      for (var index = startingIndex + 1; index < Ushorts.Count; index++)
        if (this.Instructions[index] == Instruction.Label)
          return this.NextLabel((ushort) index, iterations - 1);
      for (var index = 0; index < startingIndex; index++)
        if (this.Instructions[index] == Instruction.Label)
          return this.NextLabel((ushort) index, iterations - 1);

      return startingIndex;
    }
  }
}
