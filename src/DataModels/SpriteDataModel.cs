using Newtonsoft.Json;
using UnityEngine;

namespace Directers_Cut.DataModels
{
    internal class SpriteDataModelWrapper
    {
        internal SpriteDataModel Sprite { get; set; } = new();

        [JsonIgnore]
        internal string? BaseDirectory { get; set; }

        internal string TexturePath => Path.Combine(BaseDirectory!, Sprite.TextureName!);

        internal string TextureNameWithoutExtension => Sprite.TextureName!.Split(".")[0];

        internal string SpriteNameWithoutExtension => Sprite.SpriteName!.Split(".")[0];
    }

    internal class SpriteDataModel
    {
        [JsonProperty("rect")]
        public Rect Rect { get; set; }

        [JsonProperty("spriteName")]
        public string? SpriteName { get; set; }

        [JsonProperty("textureName")]
        public string? TextureName { get; set; }
    }
}
