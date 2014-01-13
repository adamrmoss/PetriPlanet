using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetriPlanet.Core.Organisms
{
  public class Biomass
  {
    public ushort X { get; private set; }
    public ushort Y { get; private set; }
    public ushort Energy { get; private set; }

    public bool AbsorbEnergy(ushort energy)
    {
      var oldEnergyLevel = this.Energy;
      this.Energy += energy;
      return this.Energy < oldEnergyLevel;
    }

    public bool IsFood
    {
      get { return Primes.LookupTable[this.Energy]; }
    }

    public bool IsPoison
    {
      get { return !this.IsFood; }
    }

    public Biomass(ushort x, ushort y, ushort energy)
    {
      this.X = x;
      this.Y = y;
      this.Energy = energy;
    }
  }
}
