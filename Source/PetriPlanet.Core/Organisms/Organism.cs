using System;
using System.Collections.Generic;
using System.Linq;
using PetriPlanet.Core.Collections;
using PetriPlanet.Core.Experiments;
using PetriPlanet.Core.Maths;

namespace PetriPlanet.Core.Organisms
{
  public class Organism
  {
    private static readonly Instruction[] allInstructions = EnumerableExtensions.GetAllEnumValues<Instruction>();

    private readonly Experiment experiment;

    public Guid Id { get; private set; }
    public int Generation { get; private set; }
    public int X { get; set; }
    public int Y { get; set; }
    public Direction Direction { get; private set; }

    public double Red { get; private set; }
    public double Green { get; private set; }
    public double Blue { get; private set; }

    public double Health { get; private set; }
    public double Injury { get; private set; }
    public double Steering { get; private set; }
    public double Motor { get; private set; }
    public double Aggression { get; private set; }
    public double Reproduction { get; private set; }

    public double SensedLeftHealth { get; private set; }
    public double SensedFrontHealth { get; private set; }
    public double SensedRightHealth { get; private set; }
    public double SensedLeftAggression { get; private set; }
    public double SensedFrontAggression { get; private set; }
    public double SensedRightAggression { get; private set; }
    public double SensedFrontReproduction { get; private set; }
    public double SensedLight { get; private set; }

    public Instruction[] SteeringInstructions { get; private set; }
    public Instruction[] MotorInstructions { get; private set; }
    public Instruction[] AggressionInstructions { get; private set; }
    public Instruction[] ReproductionInstructions { get; private set; }

    public Organism(Experiment experiment, OrganismSetup organismSetup)
    {
      this.experiment = experiment;

      this.Id = organismSetup.Id;
      this.Generation = organismSetup.Generation;
      this.X = organismSetup.X;
      this.Y = organismSetup.Y;
      this.Direction = organismSetup.Direction;

      this.Red = organismSetup.Red;
      this.Green = organismSetup.Green;
      this.Blue = organismSetup.Blue;

      this.Health = organismSetup.Health;
      this.Steering = organismSetup.Steering;
      this.Motor = organismSetup.Motor;
      this.Aggression = organismSetup.Aggression;
      this.Reproduction = organismSetup.Reproduction;

      this.SteeringInstructions = organismSetup.SteeringInstructions;
      this.MotorInstructions = organismSetup.MotorInstructions;
      this.AggressionInstructions = organismSetup.AggressionInstructions;
      this.ReproductionInstructions = organismSetup.ReproductionInstructions;
    }

    public OrganismSetup Save()
    {
      return new OrganismSetup {
        Id = this.Id,
        Generation = this.Generation,
        X = this.X,
        Y = this.Y,
        Direction = this.Direction,

        Red = this.Red,
        Green = this.Green,
        Blue = this.Blue,

        Health = this.Health,
        Steering = this.Steering,
        Motor = this.Motor,
        Aggression = this.Aggression,
        Reproduction = this.Reproduction,

        SteeringInstructions = this.SteeringInstructions,
        MotorInstructions = this.MotorInstructions,
        AggressionInstructions = this.AggressionInstructions,
        ReproductionInstructions = this.ReproductionInstructions,
      };
    }

    public void Tick()
    {
      this.Sense();
      this.Think();
      this.Act();
    }

    private void Sense()
    {
      var position = Tuple.Create(this.X, this.Y);
      var frontPosition = Position.FollowDirection(position, this.Direction, this.experiment.Width, this.experiment.Height);
      var leftPosition = Position.FollowDirection(frontPosition, this.Direction.TurnLeft(), this.experiment.Width, this.experiment.Height);
      var rightPosition = Position.FollowDirection(frontPosition, this.Direction.TurnRight(), this.experiment.Width, this.experiment.Height);

      var frontNeighbor = this.experiment.Organisms[frontPosition.Item1, frontPosition.Item2];
      var leftNeighbor = this.experiment.Organisms[leftPosition.Item1, leftPosition.Item2];
      var rightNeighbor = this.experiment.Organisms[rightPosition.Item1, rightPosition.Item2];

      this.SensedFrontHealth = frontNeighbor == null ? 0 : frontNeighbor.Health;
      this.SensedFrontAggression = frontNeighbor == null ? 0 : frontNeighbor.Aggression;
      this.SensedFrontReproduction = frontNeighbor == null ? 0 : frontNeighbor.Reproduction;

      this.SensedLeftHealth = leftNeighbor == null ? 0 : leftNeighbor.Health;
      this.SensedLeftAggression = leftNeighbor == null ? 0 : leftNeighbor.Aggression;

      this.SensedRightHealth = rightNeighbor == null ? 0 : rightNeighbor.Health;
      this.SensedRightAggression = rightNeighbor == null ? 0 : rightNeighbor.Aggression;


      //var isInLight = this.X >= this.experiment.SunX && this.X < this.experiment.SunX + this.experiment.SunSize &&
      //                this.Y >= this.experiment.
      //this.SensedLight
    }

    private void Think()
    {
      var steeringInterpreter = new InstructionInterpreter(this, this.experiment.Random);
      var steering = steeringInterpreter.ExecuteInstructions(this.SteeringInstructions);
      var motorInterpreter = new InstructionInterpreter(this, this.experiment.Random);
      var motor = motorInterpreter.ExecuteInstructions(this.MotorInstructions);
      var aggressionInterpreter = new InstructionInterpreter(this, this.experiment.Random);
      var aggression = aggressionInterpreter.ExecuteInstructions(this.AggressionInstructions);
      var reproductionInterpreter = new InstructionInterpreter(this, this.experiment.Random);
      var reproduction = reproductionInterpreter.ExecuteInstructions(this.ReproductionInstructions);

      this.Steering = steering;
      this.Motor = motor;
      this.Aggression = aggression;
      this.Reproduction = reproduction;

      this.Injury = 0.0;
    }

    private void Act()
    {
      var energyConsumed = 0.0;
      // TODO: Check for Reproduction and possibly reproduce-and-end-turn
      // TODO: Check for movement
      // TODO: If move, check destination.  Combat if required.  Otherwise move successful.
      // TODO: Deduct energy.
      throw new NotImplementedException();
    }

    //public void DeductEnergy(double energy)
    //{
    //  this.Health = (this.Health - energy).Latch();
    //}

    //public void AbsorbEnergy(double energy)
    //{
    //  this.Health = (this.Health + energy).Latch();
    //}
  }
}
