namespace PetriPlanet.Core.Organisms
{
  public class Organism
  {
    public Computer Computer { get; private set; }

    public float Energy { get; set; }
    public Direction Direction { get; set; }

    public Organism()
    {
      this.Computer = new Computer();
    }
  }
}
