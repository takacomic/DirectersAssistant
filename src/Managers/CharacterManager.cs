using Directers_Cut.FileModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Directers_Cut.Managers
{
    internal class CharacterManager : BaseManager
    {
        bool SpriteRetrigger = false;
        internal CharacterManager(string CharacterPath, BaseManager baseManager, SpriteManager spriteManager)
        {
            this.CharacterPath = CharacterPath;
            this.SpriteManager = spriteManager;
            _BaseManager = baseManager;
            ParseCharacterFiles();
        }

        void ParseCharacterFiles()
        {
            foreach (string dir in Directory.GetDirectories(CharacterPath))
            {
                var files = Directory.GetFiles(dir, "*.json");
                if (files.Length == 0)
                {
                    GetLogger().Warning($"{dir}, does not have a json file. Skipping.");
                    continue;
                }
                string jsonFile = Path.Combine(dir, files[0]);

                if (!File.Exists(jsonFile)) continue;

                GetLogger().Msg($"Loading character from {Path.GetDirectoryName(dir)}");
                string fileContent = File.ReadAllText(jsonFile);
                HandleJsonFileString(fileContent, jsonFile);
            }
            if (this.SpriteRetrigger) SpriteManager.ParseSprites();
        }

        void HandleJsonFileString(string json, string filePath)
        {
            BaseCharacterFileModel CharacterData = GetFileData(json, filePath, out Type actualType);

            _BaseManager.Characters.Add(CharacterData.GetCharacter());
        }

        BaseCharacterFileModel GetFileData(string json, string filePath, out Type actualType)
        {
            JObject jsonObj = new JObject();

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

            JToken? jToken = jsonObj["version"] ?? throw new InvalidDataException("Invalid json provided, no version string.");
            Version? version = jToken.ToObject<Version>();

            switch (version?.ToString())
            {
                case CharacterFileModelV1._version:
                    {
                        actualType = typeof(CharacterFileModelV1);
                        CharacterFileModelV1? c = JsonConvert.DeserializeObject<CharacterFileModelV1>(json);

                        if (c == null)
                        {
                            break;
                        }

                        return c;
                    }
                case "0.3":
                    {
                        this.SpriteRetrigger = true;

                        actualType = typeof(CharacterFileModelV1);
                        CharacterFileModelV1? c = JsonConvert.DeserializeObject<CharacterFileModelV1>(characterFromBloodlines(json, filePath));

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

        string characterFromBloodlines(string json, string filePath)
        {
            string spritePath = filePath.Split("Bloodlines")[0] + "Sprites" + filePath.Split("Bloodlines")[1];
            JObject modifyJson = JObject.Parse(json);
            JObject modifyCharacter = new JObject();
            JObject modifySprites = new JObject();

            modifyCharacter.Add("version", "1.0");
            modifySprites.Add("version", "1.0");

            modifyCharacter.Add("character", modifyJson["characters"][0]);
            modifySprites.Add("sprites", modifyJson["spriteData"]);

            File.WriteAllText(filePath, modifyCharacter.ToString());
            File.WriteAllText(spritePath, modifySprites.ToString());

            return JsonConvert.SerializeObject(modifyCharacter);
        }
    }
}
