using System;
using System.Reflection;
using HarmonyLib;

namespace ControlCompanyAddons.Helpers;

public static class HoardingBugEnemyControllerHelper {
    private static Type? _hoardingBugEnemyControllerType;
    private static FieldInfo? _hoarderBugAIField;

    internal static bool FetchHoardingBugEnemyControllerType() {
        if (_hoardingBugEnemyControllerType is not null) return true;

        var controlCompanyAssembly = ControlCompanyHelper.GetControlCompanyAssembly();

        if (controlCompanyAssembly is null) return false;

        foreach (var type in AccessTools.GetTypesFromAssembly(controlCompanyAssembly)) {
            if (type?.Namespace?.Contains("ControlCompany.Core.Enemy") is not true) continue;

            if (type?.Name?.Contains("HoardingBugEnemyController") is not true) continue;

            _hoardingBugEnemyControllerType = type;
            return true;
        }

        ControlCompanyAddons.Logger.LogError("Couldn't find ControlCompany 'HoardingBugEnemyController' type!");
        return false;
    }

    internal static bool FetchEnemyAIField() {
        if (_hoarderBugAIField is not null) return true;

        _hoarderBugAIField = AccessTools.DeclaredField(_hoardingBugEnemyControllerType, "hoarderBugAI");

        if (_hoarderBugAIField is not null) return true;

        ControlCompanyAddons.Logger.LogError("Couldn't find ControlCompany 'hoarderBugAI' type!");
        return false;
    }

    public static bool IsHoardingBugController(object? enemyController) {
        if (enemyController is null) return false;

        if (!ControlCompanyHelper.FetchControlCompanyAssembly()) return false;

        if (!FetchHoardingBugEnemyControllerType()) return false;

        if (_hoardingBugEnemyControllerType is null) return false;

        return enemyController.GetType() == _hoardingBugEnemyControllerType;
    }

    public static HoarderBugAI? GetHoardingBugAI(object enemyController) {
        if (!ControlCompanyHelper.FetchControlCompanyAssembly()) return null;

        if (!FetchHoardingBugEnemyControllerType()) return null;

        if (!FetchEnemyAIField()) return null;

        var enemyAI = _hoarderBugAIField?.GetValue(enemyController);

        return enemyAI is not HoarderBugAI hoardingBugAI? null : hoardingBugAI;
    }

    public static Type? GetHoardingBugEnemyControllerType() {
        if (!ControlCompanyHelper.FetchControlCompanyAssembly()) return null;

        return !FetchHoardingBugEnemyControllerType()? null : _hoardingBugEnemyControllerType;
    }
}