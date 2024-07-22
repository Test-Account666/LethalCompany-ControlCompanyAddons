using System;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace ControlCompanyAddons.Helpers;

public static class EnemyControllerHelper {
    private static Type? _enemyControllerType;
    private static FieldInfo? _isAIControlledField;
    private static FieldInfo? _enemyGameObjectField;
    private static MethodInfo? _enableAIControlMethod;
    private static MethodInfo? _destroyAndCleanUpMethod;

    internal static bool FetchEnemyControllerType() {
        if (_enemyControllerType is not null) return true;

        var controlCompanyAssembly = ControlCompanyHelper.GetControlCompanyAssembly();

        if (controlCompanyAssembly is null) return false;

        foreach (var type in AccessTools.GetTypesFromAssembly(controlCompanyAssembly)) {
            if (type?.Namespace?.Contains("ControlCompany.Core") is not true) continue;

            if (type?.Name?.Contains("EnemyController") is not true) continue;

            _enemyControllerType = type;
            return true;
        }

        ControlCompanyAddons.Logger.LogError("Couldn't find ControlCompany 'EnemyController' type!");
        return false;
    }

    internal static bool FetchIsAIControlledField() {
        if (_isAIControlledField is not null) return true;

        _isAIControlledField = AccessTools.DeclaredField(_enemyControllerType, "isAIControlled");

        if (_isAIControlledField is not null) return true;

        ControlCompanyAddons.Logger.LogError("Couldn't find ControlCompany 'EnemyController' isAIControlled field!");
        return false;
    }

    internal static GameObject? GetEnemyGameObject(object enemyController) {
        if (!ControlCompanyHelper.FetchControlCompanyAssembly()) return null;

        if (!FetchEnemyControllerType()) return null;

        if (!FetchEnemyGameObjectField()) return null;

        var enemyGameObject = _enemyGameObjectField?.GetValue(enemyController);

        return enemyGameObject as GameObject;
    }

    internal static bool IsAIControlled(object enemyController) {
        if (!ControlCompanyHelper.FetchControlCompanyAssembly()) return true;

        if (!FetchEnemyControllerType()) return true;

        if (!FetchIsAIControlledField()) return true;

        var isAIControlled = _isAIControlledField?.GetValue(enemyController);

        if (isAIControlled is not bool) return true;

        return isAIControlled is true;
    }

    public static Type? GetEnemyControllerType() {
        if (!ControlCompanyHelper.FetchControlCompanyAssembly()) return null;

        return !FetchEnemyControllerType()? null : _enemyControllerType;
    }

    internal static bool FetchEnemyGameObjectField() {
        if (_enemyGameObjectField is not null) return true;

        if (!FetchEnemyControllerType()) return false;

        var enemyControllerType = GetEnemyControllerType();

        if (enemyControllerType is null) return false;

        _enemyGameObjectField = AccessTools.DeclaredField(enemyControllerType, "enemyGameObject");

        if (_enemyGameObjectField is not null) return true;

        ControlCompanyAddons.Logger.LogError("Couldn't find ControlCompany 'CustomPlayerController' enemyGameObject field!");
        return false;
    }

    internal static bool FetchEnableAIControlMethod() {
        if (_enableAIControlMethod is not null) return true;

        if (!FetchEnemyControllerType()) return false;

        var enemyControllerType = GetEnemyControllerType();

        if (enemyControllerType is null) return false;

        _enableAIControlMethod = AccessTools.DeclaredMethod(enemyControllerType, "EnableAIControl", [
            typeof(bool),
        ]);

        if (_enableAIControlMethod is not null) return true;

        ControlCompanyAddons.Logger.LogError("Couldn't find ControlCompany 'CustomPlayerController' EnableAIControl method!");
        return false;
    }

    public static void EnableAIControl(object enemyController, bool enable) {
        if (!FetchEnableAIControlMethod()) return;

        _enableAIControlMethod?.Invoke(enemyController, [
            enable,
        ]);
    }

    internal static bool FetchDestroyAndCleanUpMethod() {
        if (_destroyAndCleanUpMethod is not null) return true;

        if (!FetchEnemyControllerType()) return false;

        var enemyControllerType = GetEnemyControllerType();

        if (enemyControllerType is null) return false;

        _destroyAndCleanUpMethod = AccessTools.DeclaredMethod(enemyControllerType, "DestroyAndCleanUp");

        if (_destroyAndCleanUpMethod is not null) return true;

        ControlCompanyAddons.Logger.LogError("Couldn't find ControlCompany 'EnemyController' DestroyAndCleanUp method!");
        return false;
    }

    public static void DestroyAndCleanUp(object enemyController) {
        if (!FetchDestroyAndCleanUpMethod()) return;

        _destroyAndCleanUpMethod?.Invoke(enemyController, [
        ]);
    }
}