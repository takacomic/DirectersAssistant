﻿using Directer_Machine.FileModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Directer_Machine.Managers
{
    internal class CharacterManager : BaseManager
    {
        internal CharacterManager(string CharacterPath, BaseManager baseManager)
        {
            this.CharacterPath = CharacterPath;
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
                default:
                    throw new InvalidDataException($"Invalid version number found in json string {(version == null ? "null" : version.ToString())} in <{filePath}>.");
            }

            throw new InvalidDataException($"Invalid Json object in file <{filePath}>");
        }
    }
}
