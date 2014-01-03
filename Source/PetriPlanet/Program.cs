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

      var simulation = new Experiment(width, height);
      simulation.Start();

      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);

      var experimentForm = new ExperimentForm(simulation);
      Application.Run(experimentForm);
    }
  }
}
