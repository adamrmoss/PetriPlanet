using System;
using System.Windows.Forms;
using PetriPlanet.Core;
using PetriPlanet.Core.Experiments;

namespace PetriPlanet
{
  internal static class Program
  {
    private const int width = 64;
    private const int height = 48;

    [STAThread]
    internal static void Main()
    {
      var lookupTable = Primes.LookupTable;

      var simulation = Experiment.Build(width, height);
      var simulationController = ExperimentController.Build(simulation);

      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);

      var experimentForm = new ExperimentForm().SetController(simulationController);
      experimentForm.Start();
      Application.Run(experimentForm);
    }
  }
}
