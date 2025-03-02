using Directers_Cut.FileModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Directers_Cut.Managers
{
    internal class MusicManager : BaseManager
    {
        internal MusicManager(string MusicPath, BaseManager baseManager)
        {
            this.MusicPath = MusicPath;
            _BaseManager = baseManager;
            ParseMusic();
        }

        internal void ParseMusic()
        {
            foreach (string dir in Directory.GetDirectories(MusicPath))
            {
                var files = Directory.GetFiles(dir, "*.json");
                if (files.Length == 0)
                {
                    GetLogger().Warning($"{dir}, does not have a json file. Skipping.");
                    continue;
                }
                string jsonFile = Path.Combine(dir, files[0]);

                if (!File.Exists(jsonFile)) continue;

                GetLogger().Msg($"Loading Music Data from {Path.GetDirectoryName(dir)}");
                string fileContent = File.ReadAllText(jsonFile);
                HandleJsonFileString(fileContent, jsonFile);
            }
        }

        void HandleJsonFileString(string json, string filePath)
        {
#pragma warning disable S1481
            BaseMusicFileModel MusicData = GetFileData(json, filePath, out Type actualType);
#pragma warning restore S1481

            MusicData.GetMusicList().ForEach(data =>
            {
                data.BaseDirectory = Path.GetDirectoryName(filePath);
                _BaseManager.Music.Add(data);
            });
        }

        BaseMusicFileModel GetFileData(string json, string filePath, out Type actualType)
        {
            JObject jsonObj;

            try
            {
                jsonObj = JObject.Parse(json);
            }
            catch (JsonReaderException ex)
            {
                GetLogger().Error($"** NOTE: This is unlikely an issue with '{ModInfo.Name}' **");
                GetLogger().Error("Verify that the json file has valid a json body in it!");
                GetLogger().Error("Use a tool like https://jsonlint.com to verify your json file.");
                GetLogger().Error($"Failed to parse json to JObject. Invalid Json. {ex}");
                throw new InvalidDataException("Failed to parse json to JObject", ex);
            }

            var jToken = jsonObj[propertyName: "version"] ?? throw new InvalidDataException("Invalid json provided, no version string.");
            var version = jToken.ToObject<Version>();

            switch (version?.ToString())
            {
                case MusicFileModelV1._version:
                    {
                        actualType = typeof(MusicFileModelV1);
                        var c = JsonConvert.DeserializeObject<MusicFileModelV1>(json);

                        if (c == null)
                        {
                            break;
                        }

                        return c;
                    }
                default:
                    throw new InvalidDataException($"Invalid version number found in json string {(version == null ? "null" : version.ToString())} in <{filePath}>.");
            }

            throw new InvalidDataException($"Invalid Json object in file <{filePath}>");
        }
    }
}
