using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Il2CppSystem.Runtime.Remoting.Messaging;
using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Framework;
using Il2CppVampireSurvivors.Objects.Characters;
using Il2CppVampireSurvivors.Objects;
using MelonLoader;
using Newtonsoft.Json;
using Directers_Assistant.src.Managers;

namespace Directers_Assistant.src.PatchFarm
{
#pragma warning disable S1118
    internal class BasePatch
    {
        internal static readonly JsonSerializerSettings SerializerSettings = new()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore
        };

        internal static MelonLogger.Instance GetLogger() => Melon<DirecterAssistantMod>.Logger;
        internal static BaseManager? GetManager() => Melon<DirecterAssistantMod>.Instance.BaseManager;
        internal static GameManager? _GameManager;

        internal static BasePatch? GetBasePatch() => Melon<DirecterAssistantMod>.Instance.BasePatch;
        internal static SkinType _skinType;

        internal static bool IsCustomCharacter(CharacterType characterType)
        {
            return GetManager()!.CharacterDict.ContainsKey(characterType);
        }

        internal static void HiddenWeaponLeveler(WeaponType weaponType, CharacterController characterController, bool allowMulti = false)
        {
            if (AlreadyHiddenWeapon(weaponType, characterController) && !allowMulti)
                foreach (Equipment equipment in characterController.WeaponsManager.HiddenEquipment)
                {
                    if (equipment.Type != weaponType) continue;
                    equipment.LevelUp(true);
                    break;

                }
            else
                _GameManager!.AddHiddenWeapon(weaponType, characterController, allowMulti);
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
            _GameManager!.ArcanaManager.ActiveArcanas.Add(arcanaType);
            _GameManager.ArcanaManager.TriggerArcana(arcanaType);
        }
    }
}
