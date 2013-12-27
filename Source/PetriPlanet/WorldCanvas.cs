using System.Drawing;
using System.Windows.Forms;

namespace PetriPlanet
{
  public class WorldCanvas : Control
  {
    private const int worldGridSize = 32;

    public WorldCanvas(int width, int height)
    {
      this.Width = width * worldGridSize;
      this.Height = height * worldGridSize;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      base.OnPaint(e);

      var pen = new Pen(Color.DarkGray);
      e.Graphics.DrawRectangle(pen, 0, 0, this.Width, this.Height);
    }
  }
}
