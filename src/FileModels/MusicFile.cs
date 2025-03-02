using Directers_Cut.DataModels;
using Directers_Cut.JsonModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Directers_Cut.FileModels
{
    internal abstract class BaseMusicFileModel
    {
        [JsonProperty("version")]
        [JsonConverter(typeof(VersionConverter))]
        internal abstract Version Version { get; }

        internal abstract Type FileVersion();

        internal abstract List<MusicDataModelWrapper> GetMusicList();
    }
    internal class MusicFileModelV1 : BaseMusicFileModel
    {
        [JsonIgnore]
        internal const string _version = "1.0";

        internal override Version Version { get; } = new Version("1.0");

        internal override Type FileVersion() => typeof(MusicFileModelV1);

        [JsonProperty("music")]
        internal List<MusicJsonModelV1>? Music { get; set; }

        internal override List<MusicDataModelWrapper> GetMusicList()
        {
            List<MusicDataModelWrapper> MusicData = new();

            Music!.ForEach((c) => MusicData.Add(c.toMusicDataModel()));

            return MusicData;
        }
    }
}
