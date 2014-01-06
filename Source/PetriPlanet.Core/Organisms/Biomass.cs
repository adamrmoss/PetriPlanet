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

    public ushort Value { get; private set; }

    public bool IsFood
    {
      get { return Primes.LookupTable[this.Value]; }
    }

    public bool IsPoison
    {
      get { return !this.IsFood; }
    }

    public Biomass(ushort x, ushort y, ushort value)
    {
      this.X = x;
      this.Y = y;
      this.Value = value;
    }
  }
}
