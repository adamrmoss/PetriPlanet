using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetriPlanet.Core.Organisms
{
  public enum Instruction : byte
  {
    Nop,
    IfNot0,
    IfNotEq,
    JumpBackward,
    JumpForward,
    ShiftLeft,
    ShiftRight,
    Increment,
    Decrement,
    Clear,
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
