using UnityEngine;
using MelonLoader;
using System.Reflection;

namespace Directers_Assistant.src.Textures
{
    internal class SpriteImporter : MonoBehaviour
    {
        internal static Dictionary<string, Texture2D> textures = new();
        //Taken from sup3p's Pokesurvivors
        public static Texture2D LoadTexture(string resourceName)
        {

            // Get the assembly where the resource is embedded
            Assembly assembly = Assembly.GetExecutingAssembly();

            // Format the full resource name: typically "Namespace.FolderName.FileName"
            string resourcePath = $"Directers_Assistant.src.Textures.{resourceName}";

            using (Stream stream = assembly.GetManifestResourceStream(resourcePath))
            {
                if (stream == null)
                {
                    Melon<DirecterAssistantMod>.Logger.Msg($"Resource {resourcePath} not found.");
                    Debug.LogError($"Resource {resourcePath} not found.");
                    return null;
                }

                // Read the image data from the stream
                byte[] imageData = new byte[stream.Length];
                stream.Read(imageData, 0, imageData.Length);

                // Load image data into a Texture2D
                Texture2D texture = new Texture2D(2, 2);

                if (!ImageConversion.LoadImage(texture, imageData))
                {
                    throw new Exception("ImageConversion.LoadImage failed");
                }

                texture.filterMode = FilterMode.Point;
                texture.name = resourceName;

                return texture;
            }
        }
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
