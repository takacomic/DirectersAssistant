using System.Diagnostics.CodeAnalysis;
using Directers_Assistant.src.FileModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Directers_Assistant.src.Managers
{
    internal class SpriteManager : BaseManager
    {
        internal SpriteManager(string SpritePath, BaseManager baseManager)
        {
            this.SpritePath = SpritePath;
            _BaseManager = baseManager;
            ParseSprites();
        }

        internal void ParseSprites()
        {
            foreach (string dir in Directory.GetDirectories(SpritePath))
            {
                var files = Directory.GetFiles(dir, "*.json");
                if (files.Length == 0)
                {
                    GetLogger().Warning($"{dir}, does not have a json file. Skipping.");
                    continue;
                }
                string jsonFile = Path.Combine(dir, files[0]);

                if (!File.Exists(jsonFile)) continue;

                GetLogger().Msg($"Loading Sprite Data from {Path.GetDirectoryName(files[0])}");
                string fileContent = File.ReadAllText(jsonFile);
                HandleJsonFileString(fileContent, jsonFile);
            }
        }

        void HandleJsonFileString(string json, string filePath)
        {
#pragma warning disable S1481
            BaseSpriteFileModel SpriteData = GetFileData(json, filePath, out Type actualType);
#pragma warning restore S1481

            SpriteData.GetSpriteList().ForEach(data =>
            {
                data.BaseDirectory = Path.GetDirectoryName(filePath);
                _BaseManager.Sprites.Add(data);
            });
        }

        BaseSpriteFileModel GetFileData(string json, string filePath, out Type actualType)
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
                case SpriteFileModelV1._version:
                    {
                        actualType = typeof(SpriteFileModelV1);
                        var c = JsonConvert.DeserializeObject<SpriteFileModelV1>(json);

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
