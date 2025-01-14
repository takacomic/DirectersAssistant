﻿using Directer_Machine.DataModels;
using Directer_Machine.JsonModels;
using HarmonyLib;
using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Framework;
using Il2CppVampireSurvivors.Objects.Characters;

namespace Directer_Machine.PatchFarm
{
    internal class GameManagerPatches : BasePatch
    {
        [HarmonyPatch(typeof(GameManager))]
        class GameManagerPatch
        {
            [HarmonyPatch(nameof(Il2CppVampireSurvivors.Framework.GameManager.InitializeGameSessionPostLoad))]
            [HarmonyPostfix]
            static void InitializeGameSessionPostLoad_Patch(GameManager __instance)
            {
                GameManager = __instance;
                CharacterController characterController = __instance.PlayerOne;
                CharacterType characterType = characterController.CharacterType;

                if (!IsCustomCharacter(characterType)) return;

                CharacterDataModelWrapper modelWrapper = GetManager()!.CharacterDict[characterType];
                SkinObjectModelV1 skin = modelWrapper.Skin(characterController._currentSkinData.currentSkin)!;
                foreach (ArcanaType arcanaType in skin.StartingArcana)
                {
                    ArcanaAdder(arcanaType);
                }

                foreach (WeaponType weaponType in skin.HiddenWeapons)
                {
                    HiddenWeaponLeveler(weaponType, characterController, true);
                }
            }
        }
    }
}
