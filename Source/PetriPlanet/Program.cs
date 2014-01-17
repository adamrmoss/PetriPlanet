using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;
using PetriPlanet.Core;
using PetriPlanet.Core.Experiments;
using PetriPlanet.Core.Organisms;
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
      var experimentDirectory = Path.GetDirectoryName(experimentFilePath);
      if (experimentDirectory == null)
        throw new InvalidOperationException("experimentDirectory was null!!!");

      var json = File.ReadAllText(experimentFilePath);
      var experimentSetup = JsonConvert.DeserializeObject<ExperimentSetup>(json);
      var experiment = new Experiment(experimentSetup);

      var organismFilenames = Directory.EnumerateFiles(experimentDirectory, "*.organism").ToArray();
      var organisms = organismFilenames.Select(filename => LoadOrganism(filename, experiment));
      experiment.SetupOrganisms(organisms);

      var experimentForm = new ExperimentForm(experiment);
      experimentForm.Show();
      experimentForm.Focus();
      Application.Run(experimentForm);
    }

    private static Organism LoadOrganism(string organismFilename, Experiment experiment)
    {
      var json = File.ReadAllText(organismFilename);
      var organismSetup = JsonConvert.DeserializeObject<OrganismSetup>(json);
      return new Organism(organismSetup.Id, 1, organismSetup.X, organismSetup.Y, organismSetup.Direction, organismSetup.Energy, organismSetup.Instructions, experiment);
    }
  }
}
