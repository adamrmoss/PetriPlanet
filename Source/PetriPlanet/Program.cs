using System;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;
using PetriPlanet.Core;
using PetriPlanet.Core.Experiments;

namespace PetriPlanet
{
  internal static class Program
  {
    [STAThread]
    internal static void Main()
    {
      var lookupTable = Primes.LookupTable;

      var json = File.ReadAllText(@"ExperimentSetup.json");
      var experimentSetup = JsonConvert.DeserializeObject<ExperimentSetup>(json);
      var experiment = new Experiment(experimentSetup);

      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);

      var experimentForm = new ExperimentForm(experiment);
      Application.Run(experimentForm);
    }
  }
}
