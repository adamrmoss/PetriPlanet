namespace PetriPlanet.Core.Experiments
{
  public class ExperimentController
  {
    public Experiment Experiment { get; private set; }

    public WorldGridElement[,] GetWorldGridElements()
    {
      var width = this.Experiment.WorldGrid.GetLength(0);
      var height = this.Experiment.WorldGrid.GetLength(1);
      var elements = new WorldGridElement[width, height];

      for (var i = 0; i < width; i++)
        for (var j = 0; j < height; j++)
          elements[i, j] = WorldGridElement.Build(this.Experiment.WorldGrid[i, j]);

      return elements;
    }

    private ExperimentController()
    {
    }

    public static ExperimentController Build(Experiment experiment)
    {
      return new ExperimentController {
        Experiment = experiment,
      };
    }
  }
}
