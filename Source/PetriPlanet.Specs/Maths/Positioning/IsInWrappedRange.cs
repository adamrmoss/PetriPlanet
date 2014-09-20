using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PetriPlanet.Core.Maths;
using Xunit;
using Xunit.Should;

namespace PetriPlanet.Specs.Maths.Positioning
{
  public class IsInWrappedRange
  {
    [Fact]
    public void It_knows_4_in_4_1_of_8()
    {
      4.IsInWrappedRange(4, 1, 8).ShouldBeTrue();
    }

    [Fact]
    public void It_knows_5_not_in_4_1_of_8()
    {
      5.IsInWrappedRange(4, 1, 8).ShouldBeFalse();
    }

    [Fact]
    public void It_knows_7_in_7_2_of_8()
    {
      7.IsInWrappedRange(7, 2, 8).ShouldBeTrue();
    }

    [Fact]
    public void It_knows_0_in_7_2_of_8()
    {
      0.IsInWrappedRange(7, 2, 8).ShouldBeTrue();
    }

    [Fact]
    public void It_knows_1_not_in_7_2_of_8()
    {
      1.IsInWrappedRange(7, 2, 8).ShouldBeFalse();
    }
  }
}
