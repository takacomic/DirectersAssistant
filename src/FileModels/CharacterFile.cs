﻿using Newtonsoft.Json;
using Directers_Cut.DataModels;
using Directers_Cut.JsonModels;
using Newtonsoft.Json.Converters;

namespace Directers_Cut.FileModels
{
    internal abstract class BaseCharacterFileModel
    {
        [JsonProperty("version")]
        [JsonConverter(typeof(VersionConverter))]
        internal abstract Version Version { get; }

        internal abstract Type FileVersion();

        internal abstract CharacterDataModelWrapper GetCharacter();
    }
    internal class CharacterFileModelV1 : BaseCharacterFileModel
    {
        [JsonIgnore]
        internal const string _version = "1.0";

        internal override Version Version { get; } = new Version("1.0");

        [JsonProperty("character")]
        internal CharacterJsonModelV1? Character {  get; set; }

        internal override Type FileVersion() => typeof(CharacterFileModelV1);

        internal override CharacterDataModelWrapper GetCharacter()
        {
            return Character!.toCharacterDataModel();
        }
    }
}
