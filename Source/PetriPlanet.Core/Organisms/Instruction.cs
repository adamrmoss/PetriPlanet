using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetriPlanet.Core.Organisms
{
  public enum Instruction : byte
  {
    NopX,
    NopB,
    NopC,
    IfNot0,
    IfNotEq,
    IfOdd,
    JumpBackward,
    JumpForward,
    ShiftLeft,
    ShiftRight,
    Increment,
    Decrement,
    Clear,
    SetNumber,
    Add,
    Subtract,
    Swap,
    Sort,
    Reproduce,
    Excrete,
    Walk,
    TurnLeft,
    TurnRight,
    Sense,
    Imagine,
  }
}
