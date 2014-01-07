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
  public class ArraysSpecs : BddBase
  {
    private int[] sourceArray;
    private int[] destinationArray;

    [SetUp]
    public void SetUp()
    {
      this.sourceArray = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
      this.destinationArray = new[] { 104, 103, 102, 101, 100 };
    }

    [Test]
    public void ItCanPerformAnOrdinaryCopy()
    {
      Arrays.WrapCopy(this.sourceArray, 3, this.destinationArray, 1, 3);

      Expect(this.destinationArray, EqualTo(new[] {104, 4, 5, 6, 100}));
    }

    [Test]
    public void ItCanPerformAWrappedCopy()
    {
      Arrays.WrapCopy(this.sourceArray, 8, this.destinationArray, 3, 3);

      Expect(this.destinationArray, EqualTo(new[] {1, 103, 102, 9, 10}));
    }
  }
}
