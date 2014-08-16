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
    private readonly double feltAggression;
    private readonly double feltReproduction;
    private readonly double feltInjury;
    private readonly double feltHealth;
    private readonly double feltSteering;
    private readonly double feltMotor;
    private readonly double sensedLight;
    private readonly double sensedHealth;
    private readonly double sensedAggression;
    private readonly Random random;

    private Stack<double> stack;

    public InstructionInterpreter(double feltHealth, double feltSteering, double feltMotor, double feltAggression, double feltReproduction, double feltInjury, double sensedLight, double sensedHealth, double sensedAggression, double sensedInjury, Random random) {
      this.feltHealth = feltHealth;
      this.feltSteering = feltSteering;
      this.feltMotor = feltMotor;
      this.feltAggression = feltAggression;
      this.feltReproduction = feltReproduction;
      this.feltInjury = feltInjury;
      this.sensedLight = sensedLight;
      this.sensedHealth = sensedHealth;
      this.sensedAggression = sensedAggression;
      this.random = random;

      this.stack = new Stack<double>();
    }

    public void ExecuteInstructions(IEnumerable<Instruction> instructions)
    {
      foreach (var instruction in instructions)
        this.ExecuteInstruction(instruction);
    } 

    private void ExecuteInstruction(Instruction instruction) {
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
          this.stack.Push(random.NextDouble());
          return;
        case Instruction.FeelHealth:
          this.stack.Push(this.feltHealth);
          return;
        case Instruction.FeelSteering:
          this.stack.Push(this.feltSteering);
          return;
        case Instruction.FeelMotor:
          this.stack.Push(this.feltMotor);
          return;
        case Instruction.FeelAggression:
          this.stack.Push(this.feltAggression);
          return;
        case Instruction.FeelReproduction:
          this.stack.Push(this.feltReproduction);
          return;
        case Instruction.FeelInjury:
          this.stack.Push(this.feltInjury);
          return;
        case Instruction.SenseHealth:
          this.stack.Push(this.sensedHealth);
          return;
        case Instruction.SenseAggression:
          this.stack.Push(this.sensedAggression);
          return;
        case Instruction.SenseLight:
          this.stack.Push(this.sensedLight);
          return;
        case Instruction.Dup: {
            var tos = this.SafePop();
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

    private double SafePop() {
      if (this.stack.Empty())
        return 0.0;
      return this.stack.Pop();
    }
  }
}
