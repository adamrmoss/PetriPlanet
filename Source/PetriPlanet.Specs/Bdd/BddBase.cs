using System;
using System.Collections.Generic;
using NUnit.Framework;
using PetriPlanet.Core.Collections;

namespace PetriPlanet.Specs.Bdd
{
  [TestFixture]
  public abstract class BddBase : BddBase<BddBase> { }

  [TestFixture]
  public abstract class BddBase<TSpec> : AssertionHelper where TSpec : BddBase<TSpec>
  {
    private readonly IList<Action> beforeEaches;

    protected BddBase()
    {
      this.beforeEaches = new List<Action>();
    }

    [SetUp]
    public void RunBeforeEaches()
    {
      this.beforeEaches.Each(context => context());
    }

    protected void BeforeEach(Action context)
    {
      this.beforeEaches.Add(context);
    }
  }
}
