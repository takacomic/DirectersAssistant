using Directer_Machine.DataModels;
using Directer_Machine.Textures;
using HarmonyLib;
using Il2CppNewtonsoft.Json.Linq;
using Il2CppSystem.Reflection;
using Il2CppVampireSurvivors.App.Data;
using Il2CppVampireSurvivors.Data;
using UnityEngine;
using SpriteManager = Il2CppVampireSurvivors.Graphics.SpriteManager;

#pragma warning disable S1118
#pragma warning disable S3398

namespace Directer_Machine.PatchFarm
{
    internal class DataManagerPatches : BasePatch
    {
        internal static int CharacterIDs { get; private set; } = 10000;

        [HarmonyPatch(typeof(DataManager))]
        class DataManagerPatch
        {
            [HarmonyPatch(nameof(DataManager.LoadBaseJObjects))]
            [HarmonyPostfix]
            static void LoadBaseJObjects_Postfix(DataManager __instance, object[] __args, MethodBase __originalMethod)
            {
                SpriteRegister();
                CharacterRegister(__instance, (DlcType) 10000, null!);
            }

            [HarmonyPatch(nameof(DataManager.MergeInJsonData))]
            [HarmonyPrefix]
            static void MergeInJsonData_Patch(DataManager __instance, DataManagerSettings settings, DlcType dlcType)
            {
                CharacterRegister(__instance, dlcType, settings);
            }
        }

        static void SpriteRegister()
        {
            foreach (SpriteDataModelWrapper spriteWrapper in GetManager()!.Sprites)
            {
                if (SpriteImporter.textures.ContainsKey(spriteWrapper.TextureNameWithoutExtension))
                {
                    Sprite sprite = SpriteImporter.LoadSprite(SpriteImporter.textures[spriteWrapper.TextureNameWithoutExtension], spriteWrapper.Sprite.Rect, spriteWrapper.SpriteNameWithoutExtension);
                    SpriteManager.RegisterSprite(sprite);
                }
                else
                {
                    Texture2D texture = SpriteImporter.LoadTexture(spriteWrapper.TexturePath, spriteWrapper.TextureNameWithoutExtension);
                    Sprite sprite = SpriteImporter.LoadSprite(texture, spriteWrapper.Sprite.Rect, spriteWrapper.SpriteNameWithoutExtension);
                    SpriteManager.RegisterSprite(sprite);
                }
            }
        }

        static void CharacterRegister(DataManager __instance, DlcType dlcType, DataManagerSettings settings)
        {
            foreach (CharacterDataModelWrapper characterWrapper in GetManager()!.Characters)
            {
                CharacterDataModel character = characterWrapper.Character;
                if (dlcType != character.DlcSort) continue;
                CharacterType characterType = (CharacterType)CharacterIDs++;
                characterWrapper.CharacterType = characterType;

                GetLogger().Msg($"Adding character... {characterType} {character.CharName}");
                string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(characterWrapper.CharacterSettings,
                    Newtonsoft.Json.Formatting.Indented,
                    SerializerSettings);

                JArray json = JArray.Parse(jsonString);
                if (settings != null)
                {
                    JObject dlc = JObject.Parse(settings._CharacterDataJsonAsset.text);
                    dlc.Add(characterType.ToString(), json);
                    TextAsset textAsset = new TextAsset(dlc.ToString());
                    settings._CharacterDataJsonAsset = textAsset;
                }
                else
                    __instance._allCharactersJson.Add(characterType.ToString(), json);
                GetManager()!.CharacterDict.Add(characterType, characterWrapper);
            }
        }
    }
}
