using HarmonyLib;
using System.Reflection;
using Il2CppVampireSurvivors.Objects.Items;
using Il2CppVampireSurvivors.Objects.Pickups;
using UnityEngine;

namespace Directer_Machine.PatchFarm
{
    internal class PickupPatch : BasePatch
    {
        [HarmonyPatch(typeof(Pickup))]
        class Pickup_Patch
        {
            // Bugfix: Allows gems to be picked up, even if the character is a big boy.
            [HarmonyPatch(nameof(Pickup.GoToThePlayer))]
            [HarmonyPostfix]
            static void GoToThePlayerPostix(Gem __instance, MethodBase __originalMethod)
            {
                if (IsCustomCharacter(__instance.TargetPlayer._characterType))
                {
                    float distance = Vector2.Distance(__instance.position, __instance.TargetPlayer.position);
                    float closeEnough = 0.09f;

                    if (distance < closeEnough || float.IsInfinity(distance))
                    {
                        __instance.GetTaken();
                    }
                }
            }
        }
    }
}
