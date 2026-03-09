using UnityEngine;
using MelonLoader;
using System.Reflection;
using UnityEngine.Bindings;

namespace Directers_Assistant.src.Textures
{
    internal class SpriteImporter : MonoBehaviour
    {
        internal static Dictionary<string, Texture2D> textures = new();
        //Taken from sup3p's Pokesurvivors
        public static Texture2D LoadTexture(string resourceName)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            // Format the full resource name: typically "Namespace.FolderName.FileName"
            string resourcePath = $"Directers_Assistant.src.Textures.{resourceName}";
            using var stream = assembly.GetManifestResourceStream(resourcePath);
            if (stream == null)
            {
                Melon<DirecterAssistantMod>.Logger.Msg($"Resource {resourcePath} not found.");
                Debug.LogError($"Resource {resourcePath} not found.");
                return null;
            }
            
            var bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            var texture = new Texture2D(2, 2);
            unsafe
            {
                var ptr = UnityEngine.Object.MarshalledUnityObject.MarshalNotNull(texture);
                fixed (byte* bytesPtr = bytes)
                {
                    var managedSpanWrapper = new ManagedSpanWrapper(bytesPtr, bytes.Length);
                    ImageConversion.LoadImage_Injected(ptr, ref managedSpanWrapper, false);
                }
            }
            
            texture.name = resourceName.Split('.').First();
            texture.filterMode = FilterMode.Point;
            return texture;
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

                unsafe
                {
                    var ptr = UnityEngine.Object.MarshalledUnityObject.MarshalNotNull(texture);
                    fixed (byte* bytesPtr = imageBytes)
                    {
                        var managedSpanWrapper = new ManagedSpanWrapper(bytesPtr, imageBytes.Length);
                        ImageConversion.LoadImage_Injected(ptr, ref managedSpanWrapper, false);
                    }
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
