using UnityEngine;

namespace Directers_Cut.Textures
{
    internal class SpriteImporter : MonoBehaviour
    {
        internal static Dictionary<string, Texture2D> textures = new ();
        internal static Texture2D LoadTexture(string FilePath, string name)
        {
            Texture2D texture;

            if (!File.Exists(FilePath))
            {
                throw new ArgumentException($"FilePath does not exist. <{FilePath}>");
            }
            using (FileStream fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read))
            {
                byte[] imageBytes = new byte[fs.Length];
                fs.Read(imageBytes, 0, imageBytes.Length);
                texture = new Texture2D(2, 2);

                if (!ImageConversion.LoadImage(texture, imageBytes))
                {
                    throw new Exception("ImageConversion.LoadImage failed");
                }

                // Point makes the pixels come out much clearer.
                texture.filterMode = FilterMode.Point;
                texture.name = name;

                textures.Add(texture.name, texture);

                return texture;
            }
        }

        internal static Sprite LoadSprite(Texture2D texture, Rect rect, string name)
        {
            Sprite sprite = Sprite.Create(texture, new Rect((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height), new Vector2(0.5f, 0.5f));
            sprite.name = name;
            return sprite;
        }
    }
}
