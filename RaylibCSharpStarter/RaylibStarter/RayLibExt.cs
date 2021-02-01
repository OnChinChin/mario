using Raylib_cs;
using System.Numerics;

namespace RaylibStarter
{
    class RayLibExt
    {
        public static void DrawTexture(Texture2D texture, float xPos, float yPos, float width, float height, Color color,
            float rotation = 0.0f, float xOrigin = 0.0f, float yOrigin = 0.0f)
        {
            var dst = new Rectangle(xPos, yPos, width, height);
            var src = new Rectangle(0, 0, texture.width, texture.height);
            var origin = new Vector2(xOrigin * width, yOrigin * height);
            Raylib.DrawTexturePro(texture, src, dst, origin, rotation, color);
        }
    }
}
