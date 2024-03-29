using System;
using System.Reflection;
using HarmonyLib;

namespace ControlCompanyAddons.Helpers;

public static class MouthDogEnemyControllerHelper {
    private static Type? _mouthDogEnemyControllerType;
    private static FieldInfo? _mouthDogAIField;

    internal static bool FetchMouthDogEnemyControllerType() {
        if (_mouthDogEnemyControllerType is not null)
            return true;

        var controlCompanyAssembly = ControlCompanyHelper.GetControlCompanyAssembly();

        if (controlCompanyAssembly is null)
            return false;

        foreach (var type in AccessTools.GetTypesFromAssembly(controlCompanyAssembly)) {
            if (type?.Namespace?.Contains("ControlCompany.Core.Enemy") is not true)
                continue;

            if (type?.Name?.Contains("MouthDogEnemyController") is not true)
                continue;

            _mouthDogEnemyControllerType = type;
            return true;
        }

        ControlCompanyAddons.Logger.LogError("Couldn't find ControlCompany 'MouthDogEnemyController' type!");
        return false;
    }

    internal static bool FetchEnemyAIField() {
        if (_mouthDogAIField is not null)
            return true;

        _mouthDogAIField = AccessTools.DeclaredField(_mouthDogEnemyControllerType, "mouthDogAI");

        if (_mouthDogAIField is not null)
            return true;

        ControlCompanyAddons.Logger.LogError("Couldn't find ControlCompany 'mouthDogAI' type!");
        return false;
    }

    public static bool IsMouthDogController(object? enemyController) {
        if (enemyController is null)
            return false;

        if (!ControlCompanyHelper.FetchControlCompanyAssembly())
            return false;

        if (!FetchMouthDogEnemyControllerType())
            return false;

        if (_mouthDogEnemyControllerType is null)
            return false;

        return enemyController.GetType() == _mouthDogEnemyControllerType;
    }

    public static MouthDogAI? GetMouthDogAI(object enemyController) {
        if (!ControlCompanyHelper.FetchControlCompanyAssembly())
            return null;

        if (!FetchMouthDogEnemyControllerType())
            return null;

        if (!FetchEnemyAIField())
            return null;

        var enemyAI = _mouthDogAIField?.GetValue(enemyController);

        return enemyAI is not MouthDogAI mouthDogAI ? null : mouthDogAI;
    }

    public static Type? GetMouthDogEnemyControllerType() {
        if (!ControlCompanyHelper.FetchControlCompanyAssembly())
            return null;

        return !FetchMouthDogEnemyControllerType() ? null : _mouthDogEnemyControllerType;
    }
}