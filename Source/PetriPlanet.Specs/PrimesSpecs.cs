using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using PetriPlanet.Core;
using PetriPlanet.Specs.Bdd;

namespace PetriPlanet.Specs
{
  [TestFixture]
  public class PrimesSpecs : BddBase<PrimesSpecs>
  {
    [Test]
    public void First_few_primes()
    {
      Expect(Primes.LookupTable[0], False);
      Expect(Primes.LookupTable[1], False);
      Expect(Primes.LookupTable[2], True);
      Expect(Primes.LookupTable[3], True);
      Expect(Primes.LookupTable[4], False);
      Expect(Primes.LookupTable[5], True);
      Expect(Primes.LookupTable[6], False);
    }

    [Test]
    public void Largest_prime()
    {
      var largestPrime = GetLargestPrime();
      Expect(largestPrime, EqualTo(Primes.LargestPrime));
    }

    private static ushort GetLargestPrime()
    {
      for (var i = (ushort) (Ushorts.Count - 1); i > 2; i--) {
        if (Primes.LookupTable[i])
          return i;
      }

      return 0;
    }

    [Test]
    public void Probability_of_primes()
    {
      var primeCount = Primes.LookupTable.Where(b => b).Count();
      var primeProbability = (double) primeCount / Ushorts.Count;

      Expect(primeProbability, InRange(.0998, .0999));
    }
  }
}
