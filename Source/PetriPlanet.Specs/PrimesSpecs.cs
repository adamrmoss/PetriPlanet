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
    public void Array_size_should_be_65536()
    {
      Expect(Primes.ArraySize, EqualTo(65536));
    }
  }
}
