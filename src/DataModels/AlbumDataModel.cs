using Il2CppVampireSurvivors.Data;
using Newtonsoft.Json;
using UnityEngine;

namespace Directers_Assistant.src.DataModels
{
    internal class AlbumDataModelWrapper
    {
        internal AlbumDataModel Album { get; set; } = new();

        [JsonIgnore]
        internal string? BaseDirectory { get; set; }

    }

    internal class AlbumDataModel
    {
        [JsonProperty("contentGroupType")] public ContentGroupType ContentGroupType { get; set; } = ContentGroupType.BASE;
        [JsonProperty("icon")] public string Icon { get; set; }
        [JsonProperty("isUnlocked")] public bool IsUnlocked { get; set; }
        [JsonProperty("title")] public string Title { get; set; }
        [JsonProperty("trackList")] public List<string> TrackList { get; set; }
    }
}
