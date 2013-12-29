namespace PetriPlanet.Core.Organisms
{
  public class Organism
  {
    public float Energy { get; set; }
    public Computer Computer { get; private set; }

    public Organism()
    {
      this.Computer = new Computer();
    }
  }
}
