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
