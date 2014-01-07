using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetriPlanet.Core
{
  public static class Arrays
  {
    public static void WrapCopy<T>(T[] sourceArray, T[] destinationArray, int length)
    {
      WrapCopy(sourceArray, (ushort) sourceArray.Length, destinationArray, (ushort) destinationArray.Length, length);
    }

    public static void WrapCopy<T>(T[] sourceArray, int sourceIndex, T[] destinationArray, int destinationIndex, int length)
    {
      var tempArray = new T[length];
      var firstSourceSegmentLength = Math.Min(sourceArray.Length - sourceIndex, length);
      Array.Copy(sourceArray, sourceIndex, tempArray, 0, firstSourceSegmentLength);
      Array.Copy(sourceArray, 0, tempArray, firstSourceSegmentLength, length - firstSourceSegmentLength);

      var firstDestinationSegmentLength = Math.Min(destinationArray.Length - destinationIndex, length);
      Array.Copy(tempArray, 0, destinationArray, destinationIndex, firstDestinationSegmentLength);
      Array.Copy(tempArray, firstDestinationSegmentLength, destinationArray, 0, length - firstDestinationSegmentLength);
    }
  }
}
