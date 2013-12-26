using System.Windows.Forms;
using PetriPlanet.Core.Simulations;

namespace PetriPlanet
{
  public class SimulationForm : Form
  {
    private SimulationController controller;

    public SimulationForm SetController(SimulationController controller)
    {
      this.controller = controller;
      return this;
    }
  }
}
