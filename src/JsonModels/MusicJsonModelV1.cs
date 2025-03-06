using Directers_Assistant.src.DataModels;
using Il2CppVampireSurvivors.Data;
using Newtonsoft.Json;
using System.Reflection;

namespace Directers_Assistant.src.JsonModels
{
    internal class MusicJsonModelV1
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

        public MusicDataModelWrapper toMusicDataModel()
        {
            MusicDataModelWrapper modelWrapper = new();
            MusicDataModel s = new();
            modelWrapper.Music = s;


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
