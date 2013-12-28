using System;
using System.Drawing;

namespace PetriPlanet.Core
{
  public static class ColorExtensions
  {
    public static Color ApplyIntensity(this Color fullColor, double intensity)
    {
      if (intensity < 0 || intensity > 1)
        throw new InvalidOperationException("Intensity must be in [0, 1]");

      var alpha = fullColor.A;
      var redness = fullColor.R * intensity;
      var blueness = fullColor.B * intensity;
      var greenness = fullColor.G * intensity;

      return Color.FromArgb(alpha, (int) redness, (int) greenness, (int) blueness);
    }
  }
}
