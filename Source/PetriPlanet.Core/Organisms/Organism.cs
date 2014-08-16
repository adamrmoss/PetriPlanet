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

    public double Health { get; private set; }
    public double Steering { get; private set; }
    public double Motor { get; private set; }
    public double Aggression { get; private set; }
    public double Reproduction { get; private set; }

    public Instruction[] SteeringInstructions { get; private set; }
    public Instruction[] MotorInstructions { get; private set; }
    public Instruction[] AggressionInstructions { get; private set; }
    public Instruction[] ReproductionInstructions { get; private set; }

    public Organism(Experiment experiment, Guid id, int generation, int x, int y, Direction direction,
      double health, double steering, double motor, double aggression, double reproduction,
      Instruction[] steeringInstructions, Instruction[] motorInstructions, Instruction[] aggressionInstructions, Instruction[] reproductionInstructions)
    {
      this.experiment = experiment;

      this.Id = id;
      this.Generation = generation;
      this.X = x;
      this.Y = y;
      this.Direction = direction;

      this.Health = health;
      this.Steering = steering;
      this.Motor = motor;
      this.Aggression = aggression;
      this.Reproduction = reproduction;

      this.SteeringInstructions = steeringInstructions;
      this.MotorInstructions = motorInstructions;
      this.AggressionInstructions = aggressionInstructions;
      this.ReproductionInstructions = reproductionInstructions;
    }

    public OrganismSetup Save()
    {
      return new OrganismSetup {
        Id = this.Id,
        Generation = this.Generation,
        X = this.X,
        Y = this.Y,
        Direction = this.Direction,
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
