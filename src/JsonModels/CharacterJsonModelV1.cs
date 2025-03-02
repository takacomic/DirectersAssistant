using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Data.Characters;
using Il2CppVampireSurvivors.Objects;
using Newtonsoft.Json;
using System.Reflection;
using Directers_Cut.DataModels;
using MelonLoader;
using UnityEngine;

namespace Directers_Cut.JsonModels
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    internal class CharacterJsonModelV1
    {
        [JsonProperty("isBought")]
        public bool IsBought { get; set; }

        [JsonProperty("internalID")]
        public string InternalID { get; set; }

        [JsonProperty("statModifiers")]
        public List<StatModifierJsonModelV1>? StatModifiers { get; set; }

        [JsonProperty("alwaysHidden")]
        public bool AlwaysHidden { get; set; }

        [JsonProperty("bgm")]
        public BgmType? BGM { get; set; }

        [JsonProperty("bodyOffset")]
        public Vector2? BodyOffset { get; set; }

        [JsonProperty("charName")]
        public string? CharName { get; set; }

        [JsonProperty("currentSkin")]
        public SkinType CurrentSkin { get; set; }

        [JsonProperty("customPortrait")]
        public string? CustomPortrait { get; set; }

        [JsonProperty("customBgm")]
        public string? CustomBgm { get; set; }

        [JsonProperty("dlcSort")]
        public DlcType? DlcSort { get; set; } = (DlcType)10000;

        [JsonProperty("description")]
        public string? Description { get; set; }

        [JsonProperty("exLevels")]
        public int ExLevels { get; set; }

        [JsonProperty("exWeapons")]
        public List<string>? ExWeapons { get; set; }

        [JsonProperty("frameRate")]
        public int FrameRate { get; set; }

        [JsonProperty("headOffsets")]
        public List<Vector2>? HeadOffsets { get; set; }

        [JsonProperty("hidden")]
        public bool Hidden { get; set; }

        [JsonProperty("hiddenWeapons")]
        public List<string>? HiddenWeapons { get; set; }

        [JsonProperty("level")]
        public int Level { get; set; }

        [JsonProperty("levelUpPresets")]
        public List<Loadout>? LevelUpPresets { get; set; }

        [JsonProperty("onEveryLevelUp")]
        public StatModifierJsonModelV1? OnEveryLevelUp { get; set; }

        [JsonProperty("portraitName")]
        public string? PortraitName { get; set; }

        [JsonProperty("prefix")]
        public string? Prefix { get; set; }

        [JsonProperty("price")]
        public float Price { get; set; }

        [JsonProperty("racingOffsets")]
        public List<RacingOffsetData>? RacingOffsets { get; set; }

        [JsonProperty("requiresRelic")]
        public ItemType? RequiresRelic { get; set; }

        [JsonProperty("sizeScale")]
        public Vector2? SizeScale { get; set; }

        [JsonProperty("showcase")]
        public List<WeaponType>? Showcase { get; set; }

        [JsonProperty("skinLangSheet")]
        public string? SkinLangSheet { get; set; }

        [JsonProperty("skins")]
        public List<SkinObjectModelV1>? Skins { get; set; }

        [JsonProperty("smallPortrait")]
        public bool SmallPortrait { get; set; }

        [JsonProperty("spriteName")]
        public string? SpriteName { get; set; }

        [JsonProperty("startingWeapon")]
        public WeaponType StartingWeapon { get; set; }

        [JsonProperty("suffix")]
        public string? Suffix { get; set; }

        [JsonProperty("surname")]
        public string? Surname { get; set; }

        [JsonProperty("textureName")]
        public string? TextureName { get; set; }

        [JsonProperty("walkFrameRate")]
        public int WalkFrameRate { get; set; }

        [JsonProperty("walkingFrames")]
        public int WalkingFrames { get; set; }

        public CharacterDataModelWrapper toCharacterDataModel()
        {
            CharacterDataModelWrapper modelWrapper = new();
            CharacterDataModel c = new();
            modelWrapper.CharacterSettings.Add(c);

            StatModifierJsonModelV1 stats = StatModifiers![0];

            PropertyInfo[] statsProps = stats.GetType().GetProperties();

            List<string> statsToFloat = new() { "Revivals" };

            foreach (PropertyInfo prop in statsProps)
            {
                if (c.GetType().GetProperty(prop.Name) == null)
                {
#if DEBUG
                    Melon<DirecterMachineMod>.Logger.Msg($"No match for {prop.Name}");
#endif // DEBUG
                    continue;
                }

                var value = prop.GetValue(stats, null);

                c.GetType().GetProperty(prop.Name)!.SetValue(c,
                    statsToFloat.Contains(prop.Name) ? Convert.ToSingle(value) : value);
            }

            PropertyInfo[] myProps = GetType().GetProperties();

            foreach (PropertyInfo prop in myProps)
            {
                if (c.GetType().GetProperty(prop.Name) == null && prop.Name != "StatModifiers")
                {
#if DEBUG
                    Melon<DirecterMachineMod>.Logger.Msg($"No match for {prop.Name}");
#endif // DEBUG

                    continue;
                }

                var value = prop.GetValue(this, null);

                switch (prop.Name)
                {
                    case "StatModifiers":
                    {
                        foreach (StatModifierJsonModelV1 statMod in StatModifiers.Skip(1))
                            modelWrapper.CharacterSettings.Add(statMod.toCharacterDataModel());
                        break;
                    }
                    case "OnEveryLevelUp" when OnEveryLevelUp != null:
                        c.OnEveryLevelUp = OnEveryLevelUp.toModifierStat();
                        break;
                    default:
                        c.GetType().GetProperty(prop.Name)!.SetValue(c, value);
                        break;
                }
            }

            foreach (SkinObjectModelV1 skin in Skins)
            {
                modelWrapper.CharacterSkinIndices.Add(skin.SkinType, modelWrapper.CharacterSkinIndices.Count);
            }

            return modelWrapper;
        }
    }

    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class StatModifierJsonModelV1
    {
        [JsonProperty("amount")]
        public float Amount { get; set; }

        [JsonProperty("area")]
        public float Area { get; set; }

        [JsonProperty("armor")]
        public float Armor { get; set; }

        [JsonProperty("banish")]
        public float Banish { get; set; }

        [JsonProperty("charm")]
        public int Charm { get; set; }

        [JsonProperty("cooldown")]
        public float Cooldown { get; set; }

        [JsonProperty("curse")]
        public float Curse { get; set; }

        [JsonProperty("defang")]
        public float Defang { get; set; }

        [JsonProperty("duration")]
        public float Duration { get; set; }

        [JsonProperty("fever")]
        public float Fever { get; set; }

        [JsonProperty("greed")]
        public float Greed { get; set; }

        [JsonProperty("growth")]
        public float Growth { get; set; }

        [JsonProperty("invulTimeBonus")]
        public float InvulTimeBonus { get; set; }

        [JsonProperty("level")]
        public int Level { get; set; }

        [JsonProperty("luck")]
        public float Luck { get; set; }

        [JsonProperty("magnet")]
        public float Magnet { get; set; }

        [JsonProperty("maxHp")]
        public float MaxHp { get; set; }

        [JsonProperty("moveSpeed")]
        public float MoveSpeed { get; set; }

        [JsonProperty("power")]
        public double Power { get; set; }

        [JsonProperty("regen")]
        public float Regen { get; set; }

        [JsonProperty("rerolls")]
        public float Rerolls { get; set; }

        [JsonProperty("revivals")]
        public double Revivals { get; set; }

        [JsonProperty("shields")]
        public float Shields { get; set; }

        [JsonProperty("shroud")]
        public float Shroud { get; set; }

        [JsonProperty("skips")]
        public float Skips { get; set; }

        [JsonProperty("speed")]
        public float Speed { get; set; }

        public ModifierStats toModifierStat()
        {
            ModifierStats m = new();

            List<string> statsToFloat = new() { "Power" };

            foreach (PropertyInfo prop in GetType().GetProperties())
            {
                try
                {
                    if (m.GetType().GetProperty(prop.Name) == null) continue;

                    var value = prop.GetValue(this, null);
                    m.GetType().GetProperty(prop.Name)!.SetValue(m,
                        statsToFloat.Contains(prop.Name) ? Convert.ToSingle(value) : value);
                }
                catch (Exception e)
                {
                    e.Data.Add("CharacterDataModel.toCharacterDataModel().prop.Name", prop.Name);
                    throw;
                }
            }

            return m;
        }

        internal CharacterDataModel toCharacterDataModel()
        {
            CharacterDataModel c = new();

            PropertyInfo[] myProps = GetType().GetProperties();
            List<string> statsToFloat = new() { "Revivals" };

            foreach (PropertyInfo prop in myProps)
            {
                try
                {
                    if (c.GetType().GetProperty(prop.Name) == null)
                    {
#if DEBUG
                        Melon<DirecterMachineMod>.Logger.Msg($"No match for {prop.Name}");
#endif // DEBUG

                        continue;
                    }

                    var value = prop.GetValue(this, null);

                    c.GetType().GetProperty(prop.Name)!.SetValue(c,
                        statsToFloat.Contains(prop.Name) ? Convert.ToSingle(value) : value);
                }
                catch (Exception e)
                {
                    e.Data.Add("CharacterDataModel.toCharacterDataModel().prop.Name", prop.Name);
                    throw;
                }
            }

            return c;
        }
    }

    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class EquipmentModifierJsonModelV1
    {
        [JsonProperty("accessories")]
        public List<WeaponType> Accessories { get; set; } = new ();

        [JsonProperty("allowMultiHidden")]
        public bool AllowMulti { get; set; }

        [JsonProperty("weapons")]
        public List<WeaponType> Weapons { get; set; } = new ();

        [JsonProperty("hiddenWeapons")]
        public List<WeaponType> HiddenWeapons { get; set; } = new ();

        [JsonProperty("killCount")]
        public int KillCount { get; set; }

        [JsonProperty("level")]
        public int Level { get; set; }

        [JsonProperty("timer")]
        public int Timer { get; set; }

        [JsonProperty("arcana")]
        public List<ArcanaType> Arcana { get; set; } = new ();
    }

    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class SkinObjectModelV1
    {
        [JsonProperty("alwaysAnimated")]
        public bool AlwaysAnimated { get; set; }

        [JsonProperty("alwaysHidden")]
        public bool AlwaysHidden { get; set; }

        [JsonProperty("amount")]
        public float Amount { get; set; }

        [JsonProperty("area")]
        public float Area { get; set; }

        [JsonProperty("armor")]
        public float Armor { get; set; }

        [JsonProperty("banish")]
        public float Banish { get; set; }

        [JsonProperty("cooldown")]
        public float Cooldown { get; set; }

        [JsonProperty("curse")]
        public float Curse { get; set; }

        [JsonProperty("customStartingWeapon")]
        public string? CustomStartingWeapon { get; set; }

        [JsonProperty("description")]
        public string? Description { get; set; }

        [JsonProperty("duration")]
        public float Duration { get; set; }

        [JsonProperty("equipmentModifiers")]
        public List<EquipmentModifierJsonModelV1> EquipmentModifiers { get; set; } = new ();

        [JsonProperty("exAccessories")]
        public List<WeaponType> ExAccessories { get; set; } = new ();

        [JsonProperty("exWeapons")]
        public List<WeaponType> ExWeapons { get; set; } = new ();

        [JsonProperty("greed")]
        public float Greed { get; set; }

        [JsonProperty("growth")]
        public float Growth { get; set; }

        [JsonProperty("headOffsets")]
        public Il2CppSystem.Collections.Generic.List<Vector2>? HeadOffsets { get; set; }

        [JsonProperty("hidden")]
        public bool Hidden { get; set; }

        [JsonProperty("hiddenWeapons")]
        public List<WeaponType> HiddenWeapons { get; set; } = new ();

        [JsonProperty("luck")]
        public float Luck { get; set; }

        [JsonProperty("magnet")]
        public float Magnet { get; set; }

        [JsonProperty("maxHp")]
        public float MaxHp { get; set; }

        [JsonProperty("moveSpeed")]
        public float MoveSpeed { get; set; }

        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("onEveryLevelUp")]
        public ModifierStats? OnEveryLevelUp { get; set; }

        [JsonProperty("power")]
        public double Power { get; set; }

        [JsonProperty("prefix")]
        public string? Prefix { get; set; }

        [JsonProperty("price")]
        public float Price { get; set; }

        [JsonProperty("regen")]
        public float Regen { get; set; }

        [JsonProperty("rerolls")]
        public float Rerolls { get; set; }

        [JsonProperty("revivals")]
        public float Revivals { get; set; }

        [JsonProperty("secret")]
        public bool Secret { get; set; }

        [JsonProperty("shields")]
        public float Shields { get; set; }

        [JsonProperty("skinType")]
        public SkinType SkinType { get; set; }

        [JsonProperty("skips")]
        public float Skips { get; set; }

        [JsonProperty("speed")]
        public float Speed { get; set; }

        [JsonProperty("spriteAnims")]
        public SpriteAnims? SpriteAnims { get; set; }

        [JsonProperty("spriteName")]
        public string? SpriteName { get; set; }

        [JsonProperty("startingArcana")]
        public List<ArcanaType> StartingArcana { get; set; } = new ();

        [JsonProperty("startingWeapon")]
        public WeaponType? StartingWeapon { get; set; }

        [JsonProperty("suffix")]
        public string? Suffix { get; set; }

        [JsonProperty("textureName")]
        public string? TextureName { get; set; }

        [JsonProperty("unlocked")]
        public bool Unlocked { get; set; }

        [JsonProperty("walkingFrames")]
        public int WalkingFrames { get; set; }

        /*public static implicit operator Skin(SkinObjectModelV1 model)
        {
            Skin skin = new();

            skin.alwaysHidden = model.AlwaysHidden;
            skin.amount = model.Amount;
            skin.area = model.Area;
            skin.armor = model.Armor;
            skin.banish = model.Banish;
            skin.charSelFrame = model.CharSelFrame;
            skin.charSelTexture = model.CharSelTexture;
            skin.cooldown = model.Cooldown;
            skin.curse = model.Curse;
            skin.description = model.Description;
            skin.duration = model.Duration;
            //skin.exAccessories = exAccessories;
            //skin.exWeapons = exWeapons;
            skin.greed = model.Greed;
            skin.growth = model.Growth;
            skin.headOffsets = model.HeadOffsets;
            skin.hidden = model.Hidden;
            //skin.hiddenWeapons = hiddenWeapons;
            skin.luck = model.Luck;
            skin.magnet = model.Magnet;
            skin.maxHp = model.MaxHp;
            skin.moveSpeed = model.MoveSpeed;
            skin.name = model.Name;
            skin.onEveryLevelUp = model.OnEveryLevelUp;
            skin.power = model.Power;
            skin.prefix = model.Prefix;
            skin.price = model.Price;
            skin.regen = model.Regen;
            skin.reRolls = model.Rerolls;
            skin.revivals = model.Revivals;
            skin.secret = model.Secret;
            skin.shields = model.Shields;
            skin.skinType = model.SkinType;
            skin.skips = model.Skips;
            skin.speed = model.Speed;
            skin.spriteAnims = model.SpriteAnims;
            skin.spriteName = model.SpriteName;
            //skin.startingWeapon = model.StartingWeapon;
            skin.suffix = model.Suffix;
            skin.textureName = model.TextureName;
            skin.unlocked = model.Unlocked;
            skin.walkingFrames = model.WalkingFrames;

            return skin;
        }

        public static implicit operator SkinObjectModelV1(Skin skin)
        {
            SkinObjectModelV1 model = new();

            model.AlwaysHidden = skin.alwaysHidden;
            model.Amount = skin.amount;
            model.Area = skin.area;
            model.Armor = skin.armor;
            model.Banish = skin.banish;
            model.Cooldown = skin.cooldown;
            model.Curse = skin.curse;
            model.Description = skin.description;
            model.Duration = skin.duration;
            //model.ExAccessories = exAccessories;
            //model.ExWeapons = exWeapons;
            model.Greed = skin.greed;
            model.Growth = skin.growth;
            model.HeadOffsets = skin.headOffsets;
            model.Hidden = skin.hidden;
            //model.HiddenWeapons = hiddenWeapons;
            model.Luck = skin.luck;
            model.Magnet = skin.magnet;
            model.MaxHp = skin.maxHp;
            model.MoveSpeed = skin.moveSpeed;
            model.Name = skin.name;
            model.OnEveryLevelUp = skin.onEveryLevelUp;
            model.Power = skin.power;
            model.Prefix = skin.prefix;
            model.Price = skin.price;
            model.Regen = skin.regen;
            model.Rerolls = skin.reRolls;
            model.Revivals = skin.revivals;
            model.Secret = skin.secret;
            model.Shields = skin.shields;
            model.SkinType = skin.skinType;
            model.Skips = skin.skips;
            model.Speed = skin.speed;
            model.SpriteAnims = skin.spriteAnims;
            model.SpriteName = skin.spriteName;
            //model.StartingWeapon = skin.startingWeapon;
            model.Suffix = skin.suffix;
            model.TextureName = skin.textureName;
            model.Unlocked = skin.unlocked;
            model.WalkingFrames = skin.walkingFrames;

            return model;
        }*/
    }
}
