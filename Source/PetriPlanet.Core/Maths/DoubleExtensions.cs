using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetriPlanet.Core.Maths
{
  public static class DoubleExtensions
  {
    public static double Latch(this double value)
    {
      return
        value < 0.0 ? 0.0 :
        value > 1.0 ? 1.0 : value;
    }
  }
}
