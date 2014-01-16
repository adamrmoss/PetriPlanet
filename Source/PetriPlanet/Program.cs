using System;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;
using PetriPlanet.Core;
using PetriPlanet.Core.Experiments;
using PetriPlanet.Gui;

namespace PetriPlanet
{
  internal static class Program
  {
    [STAThread]
    internal static void Main()
    {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);

      var lookupTable = Primes.LookupTable;

      var openFileDialog = new OpenFileDialog {
        Title = "Open Experiment",
        Filter = "Experiment files (*.experiment)|*.experiment",
        RestoreDirectory = false,
      };

      var dialogResult = openFileDialog.ShowDialog();
      var experimentFilePath = openFileDialog.FileName;
      var json = File.ReadAllText(experimentFilePath);
      var experimentSetup = JsonConvert.DeserializeObject<ExperimentSetup>(json);
      var experiment = new Experiment(experimentSetup);

      var experimentForm = new ExperimentForm(experiment);
      experimentForm.Show();
      experimentForm.Focus();
      Application.Run(experimentForm);
    }
  }
}
