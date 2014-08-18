using PetriPlanet.Core.Collections;
using PetriPlanet.Specs.Bdd;
using Xunit;
using Xunit.Should;

namespace PetriPlanet.Specs.Collections
{
  public class ArraysSpecs : BddBase
  {
    private readonly int[] sourceArray;
    private readonly int[] destinationArray;

    public ArraysSpecs()
    {
      this.sourceArray = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
      this.destinationArray = new[] { 104, 103, 102, 101, 100 };
    }

    [Fact]
    public void ItCanPerformAnOrdinaryCopy()
    {
      Arrays.WrapCopy(this.sourceArray, 3, this.destinationArray, 1, 3);

      this.destinationArray.ShouldBe(new[] { 104, 4, 5, 6, 100 });
    }

    [Fact]
    public void ItCanPerformAWrappedCopy()
    {
      Arrays.WrapCopy(this.sourceArray, 8, this.destinationArray, 3, 3);

      this.destinationArray.ShouldBe(new[] { 1, 103, 102, 9, 10 });
    }
  }
}
