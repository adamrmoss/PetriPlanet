using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetriPlanet.Core
{
  public static class Primes
  {
    public static bool[] LookupTable { get; private set; }

    static Primes()
    {
      LookupTable = new bool[Ushorts.UshortCount];
      for (var i = 2; i < Ushorts.UshortCount; i++) {
        var factorLimit = (int) Math.Sqrt(i);

        var isPrime = true;
        for (var j = 2; j <= factorLimit; j++) {
          var potentialFactorIsPrime = LookupTable[j];
          if (potentialFactorIsPrime && i % j == 0)
            isPrime = false;
        }
        LookupTable[i] = isPrime;
      }
    }
  }
}
