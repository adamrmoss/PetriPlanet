using NUnit.Framework;

namespace PetriPlanet.Specs.Math
{
  [TestFixture]
  public class UShorts : AssertionHelper
  {
    [Test]
    public void It_should_wrap_around_increment_to_zero()
    {
      var num = (ushort) 0xffff;
      num++;

      this.Expect(num, this.EqualTo((ushort) 0));
    }

    [Test]
    public void It_should_wrap_around_decrement_to_ffff()
    {
      var num = (ushort) 0;
      num--;

      this.Expect(num, this.EqualTo((ushort) 0xffff));
    }

    [Test]
    public void It_should_wrap_around_cast_to_zero()
    {
      var @int = 0x10000;
      var num = (ushort) @int;

      this.Expect(num, this.EqualTo((ushort) 0));
    }

    [Test]
    public void It_should_wrap_around_cast_to_ffff()
    {
      var @int = -1;
      var num = (ushort) @int;

      this.Expect(num, this.EqualTo((ushort) 0xffff));
    }
  }
}
