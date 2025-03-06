using System.Reflection;
using Directers_Assistant.src.DataModels;
using HarmonyLib;
using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Graphics;
using Il2CppVampireSurvivors.Objects.Characters;
using Il2CppVampireSurvivors.UI;
using UnityEngine;

namespace Directers_Assistant.src.PatchFarm
{
    internal class CharacterSelectionPatches : BasePatch
    {
        [HarmonyPatch(typeof(CharacterItemUI))]
        class CharacterItemUI_Patch
        {
            [HarmonyPatch(nameof(CharacterItemUI.SetIconSizes))]
            [HarmonyPrefix]
            static bool SetCharacterSpritePatch(CharacterItemUI __instance)
            {
                CharacterType cType = __instance.CharacterItem._characterType;
                if (!IsCustomCharacter(cType)) return true;
                _skinType = __instance.CharacterItem.GetCurrentSkinItem().SkinType;

                CharacterDataModelWrapper modelWrapper = GetManager()!.CharacterDict[cType];
                if (modelWrapper.Character.CustomPortrait == null && !modelWrapper.Character.SmallPortrait)
                    return true;
                if (modelWrapper.Character.SmallPortrait)
                {
                    Vector2 size = new Vector2();
                    Sprite sprite = SpriteManager.GetSprite(modelWrapper.Skin(__instance._charItem.CharacterData.currentSkin)!.SpriteName);
                    float widthDiv = sprite.rect.width / sprite.rect.height;
                    float heightDiv = sprite.rect.height / sprite.rect.width;
                    if (widthDiv >= 1.3)
                    {
                        size.x = 135;
                        size.y = 100;
                    }
                    else if (heightDiv >= 1.3)
                    {
                        size.x = 100;
                        size.y = 135;
                    }
                    else
                    {
                        size.x = 120;
                        size.y = 120;
                    }

                    __instance._CharacterIcon.rectTransform.sizeDelta = size;
                    __instance.gameObject.GetComponent<RectTransform>().sizeDelta = size;
                }
                if (modelWrapper.Character.CustomPortrait != null)
                {
                    Sprite sprite = SpriteManager.GetSprite(modelWrapper.Character.CustomPortrait);
                    __instance._CharacterIcon.overrideSprite = sprite;
                    __instance._CharacterIcon.sprite = sprite;

                    Vector2 size = new Vector2();
                    float widthDiv = sprite.rect.width / sprite.rect.height;
                    float heightDiv = sprite.rect.height / sprite.rect.width;
                    if (widthDiv >= 1.3)
                    {
                        size.x = 135;
                        size.y = 100;
                    }
                    else if (heightDiv >= 1.3)
                    {
                        size.x = 100;
                        size.y = 135;
                    }
                    else
                    {
                        size.x = 120;
                        size.y = 120;
                    }

                    __instance._CharacterIcon.rectTransform.sizeDelta = size;
                    __instance.gameObject.GetComponent<RectTransform>().sizeDelta = size;
                }

                return false;
            }
        }

        [HarmonyPatch(typeof(CharacterSelectionPage))]
        class CharacterSelectionPagePatch
        {
            [HarmonyPatch(nameof(CharacterSelectionPage.SetIconSizes))]
            [HarmonyPrefix]
            static bool SetIconSizes_Prefix(CharacterSelectionPage __instance, MethodBase __originalMethod)
            {
                CharacterType cType = __instance._currentType;
                if (!IsCustomCharacter(cType)) return true;

                CharacterDataModelWrapper modelWrapper = GetManager()!.CharacterDict[cType];
                if (!modelWrapper.Character.SmallPortrait) return true;

                Vector2 size = new Vector2();
                Sprite sprite = SpriteManager.GetSprite(modelWrapper.Skin(__instance._currentData.currentSkin)!.SpriteName);
                float widthDiv = sprite.rect.width / sprite.rect.height;
                float heightDiv = sprite.rect.height / sprite.rect.width;
                if (widthDiv >= 1.3)
                {
                    size.x = 155;
                    size.y = 110;
                }
                else if (heightDiv >= 1.3)
                {
                    size.x = 110;
                    size.y = 155;
                }
                else
                {
                    size.x = 180;
                    size.y = 180;
                }

                __instance.gameObject.transform.FindChild("Panel").transform.FindChild("InfoPanel")
                    .transform.FindChild("Background").transform.FindChild("CharacterImage")
                    .GetComponent<RectTransform>().sizeDelta = size;
                return false;
            }
        }
    }
}
