namespace PetriPlanet.Core.Organisms
{
  public class Organism
  {
    public Computer Computer { get; private set; }

    public float Energy { get; set; }

    public Organism(Computer computer)
    {
      this.Computer = computer;
    }
  }
}
