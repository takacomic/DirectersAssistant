using Directers_Assistant.src.DataModels;
using Directers_Assistant.src.JsonModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Directers_Assistant.src.FileModels
{
    internal abstract class BaseAlbumFileModel
    {
        [JsonProperty("version")]
        [JsonConverter(typeof(VersionConverter))]
        internal abstract Version Version { get; }

        internal abstract Type FileVersion();

        internal abstract AlbumDataModelWrapper GetAlubm();
    }
    internal class AlbumFileModelV1 : BaseAlbumFileModel
    {
        [JsonIgnore]
        internal const string _version = "1.0";

        internal override Version Version { get; } = new Version("1.0");

        internal override Type FileVersion() => typeof(AlbumFileModelV1);

        [JsonProperty("album")]
        internal AlbumJsonModelV1? Album { get; set; }

        internal override AlbumDataModelWrapper GetAlubm()
        {
            return Album!.toAlbumDataModel();
        }
    }
}
