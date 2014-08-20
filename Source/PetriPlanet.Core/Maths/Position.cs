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
  }
}
