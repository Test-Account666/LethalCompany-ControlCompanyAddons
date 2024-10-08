using ControlCompany.Core;
using ControlCompanyAddons.Helpers;
using HarmonyLib;
using UnityEngine;

namespace ControlCompanyAddons.Patches.AI;

[HarmonyPatch(typeof(EnemyAI))]
public static class HoarderBugAIPatch {
    // ReSharper disable InconsistentNaming
    [HarmonyPatch(nameof(EnemyAI.SwitchToBehaviourServerRpc))]
    [HarmonyPrefix]
    public static void SwitchToBehaviourServerRpcPrefix(EnemyAI __instance, ref int stateIndex) =>
        SwitchToBehaviourStateHandler(__instance, ref stateIndex);

    [HarmonyPatch(nameof(EnemyAI.SwitchToBehaviourState))]
    [HarmonyPrefix]
    public static void SwitchToBehaviourStatePrefix(EnemyAI __instance, ref int stateIndex) =>
        SwitchToBehaviourStateHandler(__instance, ref stateIndex);

    [HarmonyPatch(nameof(EnemyAI.SwitchToBehaviourStateOnLocalClient))]
    [HarmonyPrefix]
    public static void SwitchToBehaviourStateOnLocalClientPrefix(EnemyAI __instance, ref int stateIndex) =>
        SwitchToBehaviourStateHandler(__instance, ref stateIndex);

    private static void SwitchToBehaviourStateHandler(Component enemyAI, ref int stateIndex) {
        if (enemyAI is not HoarderBugAI) return;

        if (ControlCenter.Instance.currentMode != ControlCenter.Mode.ENEMY) return;

        var currentControlledEnemy = ControlCenter.Instance.currentControlledEnemy;

        if (currentControlledEnemy == null) return;

        if (currentControlledEnemy.isAIControlled) return;

        var dataHelper = enemyAI.GetComponent<DataHelper>();

        if (dataHelper == null) return;

        if (!dataHelper.HasData("IsAttacking")) return;

        var isAttacking = (bool) dataHelper.GetData("IsAttacking");

        if (isAttacking) return;

        if (stateIndex != 2) return;

        stateIndex = 0;
    }
}