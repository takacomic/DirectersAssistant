using Directers_Assistant.src.DataModels;
using Directers_Assistant.src.JsonModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Directers_Assistant.src.FileModels
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
        internal CharacterJsonModelV1? Character { get; set; }

        internal override Type FileVersion() => typeof(CharacterFileModelV1);

        internal override CharacterDataModelWrapper GetCharacter()
        {
            return Character!.toCharacterDataModel();
        }
    }
}
