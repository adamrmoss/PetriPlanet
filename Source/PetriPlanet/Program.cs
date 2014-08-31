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
    private static Experiment experiment;
    private static ExperimentForm experimentForm;
    private static string experimentDirectory;

    [STAThread]
    internal static void Main()
    {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);

      var openFileDialog = new OpenFileDialog {
        Title = "Open Experiment",
        Filter = "Experiment files (*.experiment)|*.experiment",
        RestoreDirectory = false,
      };
      var dialogResult = openFileDialog.ShowDialog();
      var experimentFilePath = openFileDialog.FileName;
      experimentDirectory = Path.GetDirectoryName(experimentFilePath);
      if (experimentDirectory == null)
        throw new InvalidOperationException("experimentDirectory was null!!!");

      var json = File.ReadAllText(experimentFilePath);
      var experimentSetup = JsonConvert.DeserializeObject<ExperimentSetup>(json);
      experiment = new Experiment(experimentSetup);

      var organismFilenames = Directory.EnumerateFiles(experimentDirectory, "*.organism").ToArray();
      var organisms = organismFilenames.Select(LoadOrganism);
      experiment.SetupOrganisms(organisms);

      experimentForm = new ExperimentForm(experiment);
      experimentForm.FormClosing += OnExperimentFormClosing;
      experimentForm.Show();
      experimentForm.Focus();
      Application.Run(experimentForm);
    }

    private static Organism LoadOrganism(string organismFilename)
    {
      var json = File.ReadAllText(organismFilename);
      var organismSetup = JsonConvert.DeserializeObject<OrganismSetup>(json);
      return new Organism(experiment, organismSetup);
    }

    private static void OnExperimentFormClosing(object sender, FormClosingEventArgs formClosingEventArgs)
    {
      try {
        if (experiment.Population > 0) {
          var shouldSaveResult = MessageBox.Show(experimentForm, "Do you want to save the current population?", "Save?", MessageBoxButtons.YesNo);
          if (shouldSaveResult == DialogResult.Yes) {
            var existingOrganismFilenames = Directory.EnumerateFiles(experimentDirectory, "*.organism").ToArray();
            foreach (var existingOrganismFilename in existingOrganismFilenames) {
              File.Delete(existingOrganismFilename);
            }

            foreach (var organism in experiment.SetOfOrganisms) {
              var organismSetup = organism.Save();
              var json = JsonConvert.SerializeObject(organismSetup);

              var filename = string.Format("{0}.organism", organism.Id);
              var path = Path.Combine(experimentDirectory, filename);
              var fileStream = File.Open(path, FileMode.Create);
              var streamWriter = new StreamWriter(fileStream);
              streamWriter.Write(json);
              streamWriter.Flush();
              fileStream.Close();
            }
          }
        }
      } catch (Exception e) {
        Console.WriteLine(e);
      }
    }
  }
}
