using Il2CppVampireSurvivors.Data;
using Newtonsoft.Json;
using UnityEngine;
using AudioImportLib;

namespace Directers_Assistant.src.DataModels
{
    internal class MusicDataModelWrapper
    {
        internal MusicDataModel Music { get; set; } = new();

        [JsonIgnore]
        internal string? BaseDirectory { get; set; }

        internal string MusicPath => Path.Combine(BaseDirectory!, Music.Source!);

        internal AudioClip clip => API.LoadAudioClip(MusicPath);
    }

    internal class MusicDataModel
    {
        [JsonProperty("internalID")] public string InternalID { get; set; }
        [JsonProperty("author")] public string Author { get; set; }
        [JsonProperty("isUnlocked")] public bool IsUnlocked { get; set; }
        [JsonProperty("source")] public string Source { get; set; }
        [JsonProperty("unlockedByCharacter")] public string? UnlockCharacter { get; set; }
        [JsonProperty("unlockedByItem")] public string? UnlockItem { get; set; }
        [JsonProperty("unlockedByStage")] public string? UnlockStage { get; set; }
        [JsonProperty("title")] public string Title { get; set; }
        [JsonProperty("icon")] public string Icon { get; set; } = "stage_forest_A.png";
        [JsonProperty("hyperMod")] public HyperMod? HyperMod { get; set; }
        [JsonProperty("forsakenMod")] public ForsakenMod? ForsakenMod { get; set; }
    }
}
