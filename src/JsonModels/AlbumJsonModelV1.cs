using Directers_Assistant.src.DataModels;
using Il2CppVampireSurvivors.Data;
using MelonLoader;
using Newtonsoft.Json;
using System.Reflection;

namespace Directers_Assistant.src.JsonModels
{
    internal class AlbumJsonModelV1
    {
        [JsonProperty("contentGroupType")] public ContentGroupType ContentGroupType { get; set; } = ContentGroupType.BASE;
        [JsonProperty("icon")] public string Icon { get; set; }
        [JsonProperty("isUnlocked")] public bool IsUnlocked { get; set; }
        [JsonProperty("title")] public string Title { get; set; }
        [JsonProperty("trackList")] public List<string> TrackList { get; set; }

        public AlbumDataModelWrapper toAlbumDataModel()
        {
            AlbumDataModelWrapper modelWrapper = new();
            AlbumDataModel s = new();
            modelWrapper.Album = s;


            PropertyInfo[] myProps = GetType().GetProperties();

            foreach (PropertyInfo prop in myProps)
            {
                if (s.GetType().GetProperty(prop.Name) == null)
                {
#if DEBUG
                    Melon<DirecterAssistantMod>.Logger.Msg($"No match for {prop.Name}");
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
