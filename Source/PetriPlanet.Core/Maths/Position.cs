using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetriPlanet.Core.Maths
{
  public static class Position
  {
    public static Tuple<int, int> FollowDirection(Tuple<int, int> startingPosition, Direction direction, int xMax, int yMax)
    {
      var newX = (startingPosition.Item1 + direction.GetDeltaX() + xMax) % xMax;
      var newY = (startingPosition.Item2 + direction.GetDeltaY() + yMax) % yMax;
      return Tuple.Create(newX, newY);
    }

    public static bool IsInWrappedRange(this int index, int rangeStart, int rangeLength, int length)
    {
      if (index > length)
        throw new IndexOutOfRangeException("index > length");

      return (index >= rangeStart && index < Math.Min(rangeStart + rangeLength, length)) ||
             (rangeStart + rangeLength > length && index < (rangeStart + rangeLength) % length);
    }
  }
}
