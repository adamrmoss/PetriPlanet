namespace PetriPlanet.Core.Organisms
{
  public enum Instruction : byte
  {
    None,
    Some,
    Half,
    Most,
    All,
    Random,

    Health,
    Steering,
    Motor,
    Aggression,
    Reproduction,
    Injury,

    SenseLeftHealth,
    SenseFrontHealth,
    SenseRightHealth,
    SenseLeftAggression,
    SenseFrontAggression,
    SenseRightAggression,
    SenseFrontReproduction,
    SenseLight,

    Dup,
    Pick,

    Not,
    Multiply,
    Add,
    Average,
  }
}
