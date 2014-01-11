using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetriPlanet.Core.Organisms
{
  public enum Instruction : byte
  {
    Label,
    JumpBackward,
    JumpForward,
    Skip,

    IfNot0,
    IfNotEq,

    Clear,
    Swap,
    Sort,

    ShiftLeft,
    ShiftRight,
    Increment,
    Decrement,
    Add,
    Subtract,

    Push,
    Pop,
    Peek,

    Sense,
    Imagine,
    Reproduce,
    Excrete,

    Walk,
    TurnLeft,
    TurnRight,
  }

  public static class InstructionExtensions
  {
    public static ushort GetEnergyCost(this Instruction instruction)
    {
      switch (instruction) {
        case Instruction.Label:
        case Instruction.JumpBackward:
        case Instruction.JumpForward:
        case Instruction.Skip:
        case Instruction.IfNot0:
        case Instruction.IfNotEq:
        case Instruction.Clear:
        case Instruction.Swap:
        case Instruction.Sort:
        case Instruction.ShiftLeft:
        case Instruction.ShiftRight:
        case Instruction.Increment:
        case Instruction.Decrement:
        case Instruction.Add:
        case Instruction.Subtract:
        case Instruction.Push:
        case Instruction.Pop:
        case Instruction.Peek:
          return 1;
        case Instruction.Sense:
          return 2;
        case Instruction.Imagine:
          return 2;
        case Instruction.Reproduce:
          return 128;
        case Instruction.Excrete:
          return 4;
        case Instruction.Walk:
          return 8;
        case Instruction.TurnLeft:
        case Instruction.TurnRight:
          return 2;
        default:
          throw new ArgumentException("Instruction not found: " + instruction);
      }
    }
  }
}
