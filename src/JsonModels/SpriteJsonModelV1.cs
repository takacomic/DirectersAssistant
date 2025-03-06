using Newtonsoft.Json;
using System.Reflection;
using UnityEngine;
using Directers_Assistant.src.DataModels;

namespace Directers_Assistant.src.JsonModels
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    internal class SpriteJsonModelV1
    {
        [JsonProperty("rect")]
        public Rect Rect { get; set; }

        [JsonProperty("spriteName")]
        public string? SpriteName { get; set; }

        [JsonProperty("textureName")]
        public string? TextureName { get; set; }

        public SpriteDataModelWrapper toSpriteDataModel()
        {
            SpriteDataModelWrapper modelWrapper = new();
            SpriteDataModel s = new();
            modelWrapper.Sprite = s;


            PropertyInfo[] myProps = GetType().GetProperties();

            foreach (PropertyInfo prop in myProps)
            {
                if (s.GetType().GetProperty(prop.Name) == null)
                {
#if DEBUG
                    Melon<DirecterMachineMod>.Logger.Msg($"No match for {prop.Name}");
#endif // DEBUG

                    continue;
                }

                var value = prop.GetValue(this, null);
                s.GetType().GetProperty(prop.Name)!.SetValue(s, value);
            }

            return modelWrapper;
        }
    }
}
