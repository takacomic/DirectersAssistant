using Directers_Assistant.src.JsonModels;
using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Data.Characters;
using Il2CppVampireSurvivors.Objects;
using Newtonsoft.Json;
using UnityEngine;

namespace Directers_Assistant.src.DataModels
{
    internal class CharacterDataModelWrapper
    {
        [JsonIgnore]
        internal CharacterType CharacterType { get; set; }

        [JsonIgnore]
        internal List<CharacterDataModel> CharacterSettings { get; set; } = new();

        internal Dictionary<SkinType, int> CharacterSkinIndices { get; set; } = new();

        internal SkinObjectModelV1? Skin(SkinType type) => Character.Skins?[CharacterSkinIndices[type]];

        [JsonIgnore]
        internal CharacterDataModel Character
        {
            get
            {
                if (CharacterSettings.Any()) return CharacterSettings[0];
                throw new InvalidOperationException("Character have not been set.");
            }
        }
    }

    internal class CharacterDataModel
    {
        [JsonProperty("internalID")] public string InternalID { get; set; }

        [JsonProperty("isBought")] public bool IsBought { get; set; }

        [JsonProperty("alwaysHidden")] public bool AlwaysHidden { get; set; }

        [JsonProperty("amount")] public float Amount { get; set; }

        [JsonProperty("area")] public float Area { get; set; }

        [JsonProperty("armor")] public float Armor { get; set; }

        [JsonProperty("banish")] public float Banish { get; set; }

        [JsonProperty("bgm")] public BgmType? BGM { get; set; }

        [JsonProperty("customBgm")] public string? CustomBgm { get; set; }

        [JsonProperty("bodyOffset")] public Vector2 BodyOffset { get; set; }

        [JsonProperty("charm")] public int Charm { get; set; }

        [JsonProperty("charName")] public string? CharName { get; set; }

        [JsonProperty("cooldown")] public float Cooldown { get; set; }

        [JsonProperty("currentSkin")] public SkinType CurrentSkin { get; set; }

        [JsonProperty("curse")] public float Curse { get; set; }

        [JsonProperty("customPortrait")] public string? CustomPortrait { get; set; }

        [JsonIgnore] public string? CustomStartingWeapon { get; set; }

        [JsonProperty("debugEnemies")] public float DebugEnemies { get; set; }

        [JsonProperty("debugTime")] public float DebugTime { get; set; }

        [JsonProperty("defang")] public float Defang { get; set; }

        [JsonProperty("description")] public string? Description { get; set; }

        [JsonProperty("dlcSort")] public DlcType? DlcSort { get; set; } = (DlcType)10000;

        [JsonProperty("duration")] public float Duration { get; set; }

        [JsonProperty("exLevels")] public int ExLevels { get; set; }

        [JsonProperty("exWeapons")] public List<string>? ExWeapons { get; set; }

        [JsonProperty("fever")] public float Fever { get; set; }

        [JsonProperty("frameRate")] public int? FrameRate { get; set; }

        [JsonProperty("greed")] public float Greed { get; set; }

        [JsonProperty("growth")] public float Growth { get; set; }

        [JsonProperty("headOffsets")] public List<Vector2>? HeadOffsets { get; set; }

        [JsonProperty("hidden")] public bool Hidden { get; set; }

        [JsonProperty("hiddenWeapons")] public List<string>? HiddenWeapons { get; set; }

        [JsonProperty("hideWeaponIcon")] public bool HideWeaponIcon { get; set; }

        [JsonProperty("invulTimeBonus")] public float InvulTimeBonus { get; set; }

        [JsonProperty("level")] public int Level { get; set; }

        [JsonProperty("levelUpPresets")] public List<Loadout>? LevelUpPresets { get; set; }

        [JsonProperty("luck")] public float Luck { get; set; }

        [JsonProperty("magnet")] public float Magnet { get; set; }

        [JsonProperty("maxHp")] public float MaxHp { get; set; }

        [JsonProperty("moveSpeed")] public float MoveSpeed { get; set; }

        [JsonProperty("noHurt")] public bool NoHurt { get; set; }

        [JsonProperty("onEveryLevelUp")] public ModifierStats? OnEveryLevelUp { get; set; }

        [JsonProperty("portraitName")] public string? PortraitName { get; set; }

        [JsonProperty("power")] public double Power { get; set; }

        [JsonProperty("prefix")] public string? Prefix { get; set; }

        [JsonProperty("price")] public float Price { get; set; }

        [JsonProperty("racingOffsets")] public List<RacingOffsetData>? RacingOffsets { get; set; }

        [JsonProperty("regen")] public float Regen { get; set; }

        [JsonProperty("requiresRelic")] public ItemType? RequiresRelic { get; set; }

        [JsonProperty("rerolls")] public float Rerolls { get; set; }

        [JsonProperty("revivals")] public double Revivals { get; set; }

        [JsonProperty("secret")] public bool Secret { get; set; }

        [JsonProperty("shields")] public float Shields { get; set; }

        [JsonProperty("sineArea")] public SineBonusData? SineArea { get; set; }

        [JsonProperty("sineCooldown")] public SineBonusData? SineCooldown { get; set; }

        [JsonProperty("sineDuration")] public SineBonusData? SineDuration { get; set; }

        [JsonProperty("sineMight")] public SineBonusData? SineMight { get; set; }

        [JsonProperty("sineSpeed")] public SineBonusData? SineSpeed { get; set; }

        [JsonProperty("sizeScale")] public Vector2? SizeScale { get; set; }

        [JsonProperty("showcase")] public List<WeaponType>? Showcase { get; set; }

        [JsonProperty("shroud")] public float Shroud { get; set; }

        [JsonProperty("skinLangSheet")] public string? SkinLangSheet { get; set; }

        [JsonProperty("skins")] public List<SkinObjectModelV1?>? Skins { get; set; }

        [JsonProperty("skips")] public float Skips { get; set; }

        [JsonProperty("smallPortrait")] public bool SmallPortrait { get; set; }

        [JsonProperty("speed")] public float Speed { get; set; }

        [JsonProperty("spriteAnims")] public SpriteAnims? SpriteAnims { get; set; }

        [JsonProperty("spriteName")] public string? SpriteName { get; set; }

        [JsonProperty("startFrameCount")] public int StartFrameCount { get; set; }

        [JsonProperty("startingWeapon")] public WeaponType StartingWeapon { get; set; }

        [JsonProperty("suffix")] public string? Suffix { get; set; }

        [JsonProperty("surname")] public string? Surname { get; set; }

        [JsonProperty("textureName")] public string? TextureName { get; set; }

        [JsonProperty("walkFrameRate")] public int WalkFrameRate { get; set; }

        [JsonProperty("walkingFrames")] public int WalkingFrames { get; set; }

        [JsonProperty("zeroPad")] public int? ZeroPad { get; set; }
    }

}
