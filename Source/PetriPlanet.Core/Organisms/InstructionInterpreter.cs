using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PetriPlanet.Core.Collections;
using PetriPlanet.Core.Maths;

namespace PetriPlanet.Core.Organisms
{
  public class InstructionInterpreter
  {
    public Stack<double> Stack { get; private set; }

    private readonly Organism organism;
    private readonly Random random;

    public InstructionInterpreter(Organism organism, Random random)
    {
      this.organism = organism;
      this.random = random;
      this.Stack = new Stack<double>();
    }

    public void ExecuteInstructions(IEnumerable<Instruction> instructions)
    {
      foreach (var instruction in instructions)
        this.ExecuteInstruction(instruction);
    } 

    private void ExecuteInstruction(Instruction instruction) {
      switch (instruction) {
        case Instruction.None:
          this.Stack.Push(0.0);
          return;
        case Instruction.Some:
          this.Stack.Push(0.25);
          return;
        case Instruction.Half:
          this.Stack.Push(0.5);
          return;
        case Instruction.Most:
          this.Stack.Push(0.75);
          return;
        case Instruction.All:
          this.Stack.Push(1.0);
          return;
        case Instruction.Random:
          this.Stack.Push(this.random.NextDouble());
          return;
        case Instruction.Health:
          this.Stack.Push(this.organism.Health);
          return;
        case Instruction.Steering:
          this.Stack.Push(this.organism.Steering);
          return;
        case Instruction.Motor:
          this.Stack.Push(this.organism.Motor);
          return;
        case Instruction.Aggression:
          this.Stack.Push(this.organism.Aggression);
          return;
        case Instruction.Reproduction:
          this.Stack.Push(this.organism.Reproduction);
          return;
        case Instruction.Injury:
          this.Stack.Push(this.organism.Injury);
          return;
        case Instruction.SenseLeftHealth:
          this.Stack.Push(this.organism.SensedLeftHealth);
          return;
        case Instruction.SenseFrontHealth:
          this.Stack.Push(this.organism.SensedFrontHealth);
          return;
        case Instruction.SenseRightHealth:
          this.Stack.Push(this.organism.SensedRightHealth);
          return;
        case Instruction.SenseLeftAggression:
          this.Stack.Push(this.organism.SensedLeftAggression);
          return;
        case Instruction.SenseFrontAggression:
          this.Stack.Push(this.organism.SensedFrontAggression);
          return;
        case Instruction.SenseRightAggression:
          this.Stack.Push(this.organism.SensedRightAggression);
          return;
        case Instruction.SenseFrontReproduction:
          this.Stack.Push(this.organism.SensedFrontReproduction);
          return;
        case Instruction.SenseLight:
          this.Stack.Push(this.organism.SensedLight);
          return;
        case Instruction.Dup: {
            var tos = this.SafePop();
            this.Stack.Push(tos);
            this.Stack.Push(tos);
            return;
          }
        case Instruction.Pick: {
            var stackDepth = this.Stack.Count;
            var index = this.random.Next(stackDepth);
            var element = this.Stack.ElementAt(index);
            this.Stack.Clear();
            this.Stack.Push(element);
            return;
          }
        case Instruction.Not: {
          var x = this.SafePop();
          this.Stack.Push(1.0 - x);
          return;
        }
        case Instruction.Multiply: {
          var x = this.SafePop();
          var y = this.SafePop();
          this.Stack.Push(x * y);
          return;
        }
        case Instruction.Add: {
          var x = this.SafePop();
          var y = this.SafePop();
          this.Stack.Push((x + y).Latch());
          return;
        }
        case Instruction.Average: {
          var x = this.SafePop();
          var y = this.SafePop();
          this.Stack.Push((x + y) / 2.0);
          return;
        }
      }
    }

    private double SafePop() {
      if (this.Stack.Empty())
        return 0.0;
      return this.Stack.Pop();
    }
  }
}
