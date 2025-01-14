using Directer_Machine.Managers;
using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Framework;
using Il2CppVampireSurvivors.Objects.Characters;
using Il2CppVampireSurvivors.Objects;
using MelonLoader;
using Newtonsoft.Json;

namespace Directer_Machine.PatchFarm
{
#pragma warning disable S1118
    internal class BasePatch
    {
        internal static readonly JsonSerializerSettings SerializerSettings = new()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore
        };

        internal static MelonLogger.Instance GetLogger() => Melon<DirecterMachineMod>.Logger;
        internal static BaseManager? GetManager() => Melon<DirecterMachineMod>.Instance.BaseManager;
        internal static GameManager? GameManager;

        internal static bool IsCustomCharacter(CharacterType characterType)
        {
            return GetManager()!.CharacterDict.ContainsKey(characterType);
        }

        internal static void HiddenWeaponLeveler(WeaponType weaponType, CharacterController characterController, bool allowMulti)
        {
            if (AlreadyHiddenWeapon(weaponType, characterController) && !allowMulti)
                foreach (Equipment equipment in characterController.WeaponsManager.HiddenEquipment)
                {
                    if (equipment.Type == weaponType)
                    {
                        equipment.LevelUp(true);
                    }

                    break;
                }
            else
                GameManager!.AddHiddenWeapon(weaponType, characterController);
        }
        internal static bool AlreadyHiddenWeapon(WeaponType weaponType, CharacterController characterController)
        {
            foreach (Equipment equipment in characterController.WeaponsManager.HiddenEquipment)
            {
                if (equipment.Type == weaponType) return true;
            }
            return false;
        }
        internal static void ArcanaAdder(ArcanaType arcanaType)
        {
            GameManager!.ArcanaManager.ActiveArcanas.Add(arcanaType);
            GameManager.ArcanaManager.TriggerArcana(arcanaType);
        }
    }
}
