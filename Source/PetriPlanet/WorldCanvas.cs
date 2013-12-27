using System.Drawing;
using System.Windows.Forms;

namespace PetriPlanet
{
  public class WorldCanvas : Control
  {
    private const int worldGridSize = 16;

    public WorldCanvas(int width, int height)
    {
      this.Initialize(width, height);
    }

    private void Initialize(int width, int height)
    {
      this.Width = width * worldGridSize;
      this.Height = height * worldGridSize;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      base.OnPaint(e);

      var brush = new SolidBrush(Color.DarkGray);
      e.Graphics.FillRectangle(brush, 0, 0, this.Width, this.Height);
    }
  }
}
