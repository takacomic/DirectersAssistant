using System.Diagnostics;
using System.Runtime.CompilerServices;
using Directers_Cut.DataModels;
using Directers_Cut.JsonModels;
using HarmonyLib;
using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Framework;
using Il2CppVampireSurvivors.Objects.Characters;

namespace Directers_Cut.PatchFarm
{
    internal class GameManagerPatches : BasePatch
    {
        [HarmonyPatch(typeof(GameManager))]
        class GameManagerPatch
        {
            [HarmonyPatch(nameof(GameManager.InitializeGameSessionPostLoad))]
            [HarmonyPostfix]
            static void InitializeGameSessionPostLoad_Patch(GameManager __instance)
            {
                _GameManager = __instance;
                CharacterController characterController = __instance.PlayerOne;
                CharacterType characterType = characterController.CharacterType;

                if (!IsCustomCharacter(characterType)) return;

                CharacterDataModelWrapper modelWrapper = GetManager()!.CharacterDict[characterType];

                SkinObjectModelV1 skin = modelWrapper.Skin(_skinType)!;
                foreach (ArcanaType arcanaType in skin.StartingArcana)
                {
                    ArcanaAdder(arcanaType);
                }

                foreach (WeaponType weaponType in skin.HiddenWeapons)
                {
                    HiddenWeaponLeveler(weaponType, characterController);
                }
            }
        }
    }
}
