using Directer_Machine.DataModels;
using Directer_Machine.JsonModels;
using HarmonyLib;
using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Objects.Characters;
using UnityEngine;

namespace Directer_Machine.PatchFarm
{
    internal class CharacterControllerPatches : BasePatch
    {
        [HarmonyPatch(typeof(CharacterController))]
        class CharacterControllerPatch
        {
            [HarmonyPatch(nameof(CharacterController.InitCharacter))]
            [HarmonyPostfix]
            static void InitCharacter_Patch(CharacterController __instance, CharacterType characterType)
            {
                if (!IsCustomCharacter(characterType)) return;

                float x = __instance._spriteRenderer.sprite.rect.width / 95;
                x = x switch
                {
                    > 0.80f => 0.15f,
                    > 0.35f => 0.35f,
                    _ => x
                };
                Il2CppSystem.Nullable<float> a = new Il2CppSystem.Nullable<float>
                {
                    value = __instance._spriteRenderer.sprite.rect.height / 100 + 0.03f
                };
                __instance.setOrigin(x + 0.08f, a);

                // Can remove due to idle anims
                CharacterDataModelWrapper modelWrapper = GetManager()!.CharacterDict[characterType];
                if (modelWrapper.Skin(__instance.CurrentCharacterData.currentSkin)!.AlwaysAnimated)
                {
                    __instance.IsAnimForced = true;
                }
                if (modelWrapper.Character.SizeScale != null)
                {
                    Vector3 localScale = __instance.transform.localScale;
                    Vector2 sizeScale = (Vector2)modelWrapper.Character.SizeScale;
                    Vector3 newScale = new Vector3(localScale.x * sizeScale.x, localScale.y * sizeScale.y, localScale.z);
                    __instance.gameObject.transform.localScale = newScale;
                }

                if (modelWrapper.Character.FrameRate == null && modelWrapper.Character.FrameRate == 0) return;

                Il2CppSystem.Nullable<int> nullInt = new Il2CppSystem.Nullable<int>
                {
                    value = (int)modelWrapper.Character.FrameRate!
                };
                __instance.CurrentCharacterData.frameRate = nullInt;
            }

            [HarmonyPatch(nameof(CharacterController.LevelUp))]
            [HarmonyPostfix]
            static void LevelUp_Patch(CharacterController __instance)
            {
                if (!IsCustomCharacter(__instance.CharacterType)) return;

                SkinObjectModelV1 skin = GetManager()!.CharacterDict[__instance.CharacterType].Skin(__instance.CurrentSkinData.currentSkin)!;
                foreach (var modifier in skin.EquipmentModifiers.Where(modifier => modifier.Level == __instance.Level))
                {
                    foreach (WeaponType weaponType in modifier.Weapons)
                        GameManager!.AddWeapon(weaponType, __instance);

                    foreach (WeaponType weaponType in modifier.Accessories)
                        GameManager!.AccessoriesFacade.AddAccessory(weaponType, __instance);

                    foreach (WeaponType weaponType in modifier.HiddenWeapons)
                        HiddenWeaponLeveler(weaponType, __instance, modifier.AllowMulti);

                    foreach (ArcanaType arcanaType in modifier.Arcana)
                        ArcanaAdder(arcanaType);
                }
            }
        }
    }
}
