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
    private readonly Organism organism;
    private readonly Random random;
    private readonly Stack<double> stack;

    public InstructionInterpreter(Organism organism, Random random)
    {
      this.organism = organism;
      this.random = random;
      this.stack = new Stack<double>();
    }

    public double ExecuteInstructions(IEnumerable<Instruction> instructions)
    {
      foreach (var instruction in instructions)
        this.ExecuteInstruction(instruction);

      return this.stack.FirstOrDefault();
    }

    private void ExecuteInstruction(Instruction instruction)
    {
      switch (instruction) {
        case Instruction.None:
          this.stack.Push(0.0);
          return;
        case Instruction.Some:
          this.stack.Push(0.25);
          return;
        case Instruction.Half:
          this.stack.Push(0.5);
          return;
        case Instruction.Most:
          this.stack.Push(0.75);
          return;
        case Instruction.All:
          this.stack.Push(1.0);
          return;
        case Instruction.Random:
          this.stack.Push(this.random.NextDouble());
          return;
        case Instruction.Health:
          this.stack.Push(this.organism.Health);
          return;
        case Instruction.Steering:
          this.stack.Push(this.organism.Steering);
          return;
        case Instruction.Motor:
          this.stack.Push(this.organism.Motor);
          return;
        case Instruction.Aggression:
          this.stack.Push(this.organism.Aggression);
          return;
        case Instruction.Reproduction:
          this.stack.Push(this.organism.Reproduction);
          return;
        case Instruction.Injury:
          this.stack.Push(this.organism.Injury);
          return;
        case Instruction.SenseLeftHealth:
          this.stack.Push(this.organism.SensedLeftHealth);
          return;
        case Instruction.SenseFrontHealth:
          this.stack.Push(this.organism.SensedFrontHealth);
          return;
        case Instruction.SenseRightHealth:
          this.stack.Push(this.organism.SensedRightHealth);
          return;
        case Instruction.SenseLeftAggression:
          this.stack.Push(this.organism.SensedLeftAggression);
          return;
        case Instruction.SenseFrontAggression:
          this.stack.Push(this.organism.SensedFrontAggression);
          return;
        case Instruction.SenseRightAggression:
          this.stack.Push(this.organism.SensedRightAggression);
          return;
        case Instruction.SenseFrontReproduction:
          this.stack.Push(this.organism.SensedFrontReproduction);
          return;
        case Instruction.SenseLight:
          this.stack.Push(this.organism.SensedLight);
          return;
        case Instruction.Dup: {
            var tos = this.SafePeek();
            this.stack.Push(tos);
            return;
          }
        case Instruction.Pick: {
            var stackDepth = this.stack.Count;
            var index = this.random.Next(stackDepth);
            var element = this.stack.ElementAt(index);
            this.stack.Clear();
            this.stack.Push(element);
            return;
          }
        case Instruction.Not: {
            var x = this.SafePop();
            this.stack.Push(1.0 - x);
            return;
          }
        case Instruction.Multiply: {
            var x = this.SafePop();
            var y = this.SafePop();
            this.stack.Push(x * y);
            return;
          }
        case Instruction.Add: {
            var x = this.SafePop();
            var y = this.SafePop();
            this.stack.Push((x + y).Latch());
            return;
          }
        case Instruction.Average: {
            var x = this.SafePop();
            var y = this.SafePop();
            this.stack.Push((x + y) / 2.0);
            return;
          }
      }
    }

    private double SafePop()
    {
      if (this.stack.Empty())
        return 0.0;
      return this.stack.Pop();
    }

    private double SafePeek()
    {
      if (this.stack.Empty())
        return 0.0;
      return this.stack.Peek();
    }
  }
}
