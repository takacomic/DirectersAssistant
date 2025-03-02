using Directers_Cut.DataModels;
using Directers_Cut.JsonModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Directers_Cut.FileModels
{
    internal abstract class BaseSpriteFileModel
    {
        [JsonProperty("version")]
        [JsonConverter(typeof(VersionConverter))]
        internal abstract Version Version { get; }

        internal abstract Type FileVersion();

        internal abstract List<SpriteDataModelWrapper> GetSpriteList();
    }
    internal class SpriteFileModelV1 : BaseSpriteFileModel
    {
        [JsonIgnore]
        internal const string _version = "1.0";

        internal override Version Version { get; } = new Version("1.0");

        internal override Type FileVersion() => typeof(SpriteFileModelV1);

        [JsonProperty("sprites")]
        internal List<SpriteJsonModelV1>? Sprites { get; set; }

        internal override List<SpriteDataModelWrapper> GetSpriteList()
        {
            List<SpriteDataModelWrapper> SpriteData = new();

            Sprites!.ForEach((c) => SpriteData.Add(c.toSpriteDataModel()));

            return SpriteData;
        }
    }
}
