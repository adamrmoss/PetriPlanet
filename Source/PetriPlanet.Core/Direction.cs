using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetriPlanet.Core
{
  public enum Direction
  {
    East,
    North,
    West,
    South,
  }

  public static class DirectionExtensions
  {
    public static int GetDeltaX(this Direction direction)
    {
      switch (direction) {
        case Direction.East:
          return 1;
        case Direction.North:
        case Direction.South:
          return 0;
        case Direction.West:
          return -1;
        default:
          throw new InvalidOperationException(string.Format("Direction: {0} unhandled", direction));
      }
    }

    public static int GetDeltaY(this Direction direction)
    {
      switch (direction) {
        case Direction.East:
        case Direction.West:
          return 0;
        case Direction.North:
          return -1;
        case Direction.South:
          return 1;
        default:
          throw new InvalidOperationException(string.Format("Direction: {0} unhandled", direction));
      }
    }

    public static Direction TurnLeft(this Direction direction)
    {
      switch (direction) {
        case Direction.East:
          return Direction.North;
        case Direction.North:
          return Direction.West;
        case Direction.West:
          return Direction.South;
        case Direction.South:
          return Direction.East;
        default:
          throw new InvalidOperationException(string.Format("Direction: {0} unhandled", direction));
      }
    }

    public static Direction TurnRight(this Direction direction)
    {
      switch (direction) {
        case Direction.East:
          return Direction.South;
        case Direction.North:
          return Direction.East;
        case Direction.West:
          return Direction.North;
        case Direction.South:
          return Direction.West;
        default:
          throw new InvalidOperationException(string.Format("Direction: {0} unhandled", direction));
      }
    }
  }
}
