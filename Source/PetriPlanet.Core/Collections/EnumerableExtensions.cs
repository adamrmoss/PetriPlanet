using System;
using System.Collections.Generic;
using System.Linq;

namespace PetriPlanet.Core.Collections
{
  public static class EnumerableExtensions
  {
    public static void Each<T>(this IEnumerable<T> enumerable, Action<T> each)
    {
      foreach (var item in enumerable) {
        each(item);
      }
    }

    public static bool Empty<T>(this IEnumerable<T> enumerable)
    {
      return enumerable == null || !enumerable.Any();
    }

    public static IEnumerable<T> Append<T>(this IEnumerable<T> enumerable, T element)
    {
      foreach (var t in enumerable)
        yield return t;

      yield return element;
    }

    public static bool SequenceEquivalent<T>(this IEnumerable<T> first, IEnumerable<T> second)
    {
      if (first == null && second == null) return true;
      if (first == null) return false;
      if (second == null) return false;

      var firstArray = first.ToArray();
      var secondArray = second.ToArray();

      var firstCount = firstArray.Length;
      var secondCount = secondArray.Length;

      if (firstCount != secondCount) return false;

      var intersectCount = firstArray.Intersect(secondArray).Count();
      return firstCount == intersectCount;
    }

    public static HashSet<T> ToHashSet<T>(this IEnumerable<T> enumerable)
    {
      return new HashSet<T>(enumerable ?? new T[0]);
    }

    public static List<List<T>> GetCartesianProduct<T>(this List<List<T>> listOfSets)
    {
      return listOfSets.Aggregate(new List<List<T>> { new List<T>() }, CartesianProductReducer);
    }

    private static List<List<T>> CartesianProductReducer<T>(List<List<T>> partialProduct, List<T> currentSet)
    {
      return partialProduct.SelectMany(currentTuple => currentSet.Select(setItem => currentTuple.Append(setItem).ToList())).ToList();
    }

    public static IEnumerable<T> Repeat<T>(this IEnumerable<T> sequence, int times)
    {
      for (var i = 0; i < times; i++) {
        foreach (var element in sequence)
          yield return element;
      }
    }
  }
}
