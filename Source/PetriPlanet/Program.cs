using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using PetriPlanet.Core.Simulations;

namespace PetriPlanet
{
  internal static class Program
  {
    private const int width = 64;
    private const int height = 48;

    [STAThread]
    internal static void Main()
    {
      var simulation = Simulation.Build(width, height);
      var simulationController = SimulationController.Build(simulation);
      var simulationForm = new SimulationForm().SetController(simulationController);

      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      Application.Run(simulationForm);
    }
  }
}
