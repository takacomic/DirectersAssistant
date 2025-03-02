using Directers_Cut.DataModels;
using Directers_Cut.JsonModels;
using HarmonyLib;
using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Framework;
using Il2CppVampireSurvivors.Objects.Characters;
using UnityEngine;

namespace Directers_Cut.PatchFarm
{
    internal class CharacterControllerPatches : BasePatch
    {
        static List<EquipmentModifierJsonModelV1> equipmentUsed = new ();

        [HarmonyPatch(typeof(CharacterController))]
        class CharacterControllerPatch
        {
            [HarmonyPatch(nameof(CharacterController.InitCharacter))]
            [HarmonyPostfix]
            static void InitCharacter_Patch(CharacterController __instance, CharacterType characterType)
            {
                if (!IsCustomCharacter(characterType)) return;
                equipmentUsed.Clear();

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
                if (modelWrapper.Skin(_skinType)!.AlwaysAnimated)
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

                SkinObjectModelV1 skin = GetManager()!.CharacterDict[__instance.CharacterType].Skin(_skinType)!;
                foreach (var modifier in skin.EquipmentModifiers.Where(modifier => modifier.Level == __instance.Level))
                {
                    foreach (WeaponType weaponType in modifier.Weapons)
                        _GameManager!.AddWeapon(weaponType, __instance);

                    foreach (WeaponType weaponType in modifier.Accessories)
                        _GameManager!.AccessoriesFacade.AddAccessory(weaponType, __instance);

                    foreach (WeaponType weaponType in modifier.HiddenWeapons)
                        HiddenWeaponLeveler(weaponType, __instance, modifier.AllowMulti);

                    foreach (ArcanaType arcanaType in modifier.Arcana)
                        ArcanaAdder(arcanaType);
                }
            }

            [HarmonyPatch(nameof(CharacterController.OnUpdate))]
            [HarmonyPostfix]
            static void OnUpdate_Patch(CharacterController __instance)
            {
                //Wanted a nicer method to use, but couldn't find one
                KillCountEquipment(_GameManager!.MainUI.KillsText.text, __instance);
                TimerEquipment(_GameManager.SurvivedSeconds, __instance);
            }
        }

        static void KillCountEquipment(string KillCount, CharacterController characterController)
        {
            int kills = int.Parse(KillCount);
            if (!IsCustomCharacter(characterController.CharacterType)) return;

            List<EquipmentModifierJsonModelV1> equipment = GetManager()!
                .CharacterDict[characterController.CharacterType]
                .Skin(_skinType)!.EquipmentModifiers;

            if (!equipment.Any()) return;

            foreach (var modifier in equipment)
            {
                if (equipmentUsed.Contains(modifier)) continue;
                if (modifier.KillCount == 0) continue;
                if (modifier.Level != 0) continue;
                if (modifier.Timer != 0) continue;
                if (modifier.KillCount >= kills) continue;

                if (modifier.Accessories.Any())
                    foreach (var weaponType in modifier.Accessories)
                    {
                        _GameManager!.AccessoriesFacade.AddAccessory(weaponType, characterController);
                    }
                if (modifier.Weapons.Any())
                    foreach (var weaponType in modifier.Weapons)
                    {
                        _GameManager!.AddWeapon(weaponType, characterController);
                    }
                if (modifier.HiddenWeapons.Any())
                    foreach (var weaponType in modifier.HiddenWeapons)
                    {
                        HiddenWeaponLeveler(weaponType, characterController, modifier.AllowMulti);
                    }
                if (modifier.Arcana.Any())
                    foreach (var arcanaType in modifier.Arcana)
                    {
                        ArcanaAdder(arcanaType);
                    }

                equipmentUsed.Add(modifier);
            }
        }
        static void TimerEquipment(float TimerCounter, CharacterController characterController)
        {
            if (!IsCustomCharacter(characterController.CharacterType)) return;

            List<EquipmentModifierJsonModelV1> equipment = GetManager()!
                .CharacterDict[characterController.CharacterType]
                .Skin(_skinType)!.EquipmentModifiers;

            if (!equipment.Any()) return;

            foreach (var modifier in equipment)
            {
                if (equipmentUsed.Contains(modifier)) continue;
                if (modifier.KillCount != 0) continue;
                if (modifier.Level != 0) continue;
                if (modifier.Timer == 0) continue;
                if (modifier.Timer >= TimerCounter) continue;

                if (modifier.Accessories.Any())
                    foreach (var weaponType in modifier.Accessories)
                    {
                        _GameManager!.AccessoriesFacade.AddAccessory(weaponType, characterController);
                    }
                if (modifier.Weapons.Any())
                    foreach (var weaponType in modifier.Weapons)
                    {
                        _GameManager!.AddWeapon(weaponType, characterController);
                    }
                if (modifier.HiddenWeapons.Any())
                    foreach (var weaponType in modifier.HiddenWeapons)
                    {
                        HiddenWeaponLeveler(weaponType, characterController, modifier.AllowMulti);
                    }
                if (modifier.Arcana.Any())
                    foreach (var arcanaType in modifier.Arcana)
                    {
                        ArcanaAdder(arcanaType);
                    }

                equipmentUsed.Add(modifier);
            }
        }
    }
}
