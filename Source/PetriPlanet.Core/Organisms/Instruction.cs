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

    FeelHealth,
    FeelSteering,
    FeelMotor,
    FeelAggression,
    FeelReproduction,
    FeelInjury,

    SenseHealth,
    SenseAggression,
    SenseLight,

    Dup,
    Pick,

    Not,
    Multiply,
    Add,
    Average,
  }
}
