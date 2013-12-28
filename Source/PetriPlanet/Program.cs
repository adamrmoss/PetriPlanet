﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
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