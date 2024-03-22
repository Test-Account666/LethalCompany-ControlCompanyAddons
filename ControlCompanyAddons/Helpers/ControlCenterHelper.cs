using System;
using System.Reflection;
using HarmonyLib;

namespace ControlCompanyAddons.Helpers;

public static class ControlCenterHelper {
    private static Type? _controlCenterType;
    private static FieldInfo? _controlCenterInstanceField;
    private static FieldInfo? _controlCenterCurrentModeField;
    private static FieldInfo? _currentControlledEnemyField;

    internal static bool FetchControlCenterType() {
        if (_controlCenterType is not null)
            return true;

        var controlCompanyAssembly = ControlCompanyHelper.GetControlCompanyAssembly();

        if (controlCompanyAssembly is null)
            return false;

        foreach (var type in AccessTools.GetTypesFromAssembly(controlCompanyAssembly)) {
            if (type?.Namespace?.Contains("ControlCompany.Core") is not true)
                continue;

            if (type?.Name?.Contains("ControlCenter") is not true)
                continue;

            _controlCenterType = type;
            return true;
        }

        ControlCompanyAddons.Logger.LogError("Couldn't find ControlCompany 'ControlCenter' type!");
        return false;
    }

    internal static bool FetchControlCenterInstanceField() {
        if (_controlCenterInstanceField is not null)
            return true;

        _controlCenterInstanceField = AccessTools.DeclaredField(_controlCenterType, "Instance");

        if (_controlCenterInstanceField is not null)
            return true;

        ControlCompanyAddons.Logger.LogError("Couldn't find ControlCompany 'ControlCenter' instance!");
        return false;
    }

    internal static bool FetchControlCenterCurrentModeField() {
        if (_controlCenterCurrentModeField is not null)
            return true;

        _controlCenterCurrentModeField = AccessTools.DeclaredField(_controlCenterType, "currentMode");

        if (_controlCenterCurrentModeField is not null)
            return true;

        ControlCompanyAddons.Logger.LogError("Couldn't find ControlCompany 'ControlCenter' currentMode field!");
        return false;
    }

    internal static bool FetchCurrentControlledEnemyField() {
        if (_currentControlledEnemyField is not null)
            return true;

        _currentControlledEnemyField = AccessTools.DeclaredField(_controlCenterType, "currentControlledEnemy");

        if (_currentControlledEnemyField is not null)
            return true;

        ControlCompanyAddons.Logger.LogError(
            "Couldn't find ControlCompany 'ControlCenter' currentControlledEnemy field!");
        return false;
    }

    public static object? GetControlCenterInstance() {
        if (!ControlCompanyHelper.FetchControlCompanyAssembly())
            return null;

        if (!FetchControlCenterType())
            return null;

        if (!FetchControlCenterInstanceField())
            return null;

        var controlCenterInstance = _controlCenterInstanceField?.GetValue(null);

        return controlCenterInstance;
    }

    public static bool IsEnemyMode() {
        var controlCenterInstance = GetControlCenterInstance();

        if (controlCenterInstance is null)
            return false;

        if (!FetchControlCenterCurrentModeField())
            return false;

        var currentMode = _controlCenterCurrentModeField?.GetValue(controlCenterInstance);

        return currentMode is 2;
    }

    public static Type? GetControlCenterType() {
        if (!ControlCompanyHelper.FetchControlCompanyAssembly())
            return null;

        return !FetchControlCenterType() ? null : _controlCenterType;
    }

    public static object? GetCurrentControlledEnemy() {
        if (!ControlCompanyHelper.FetchControlCompanyAssembly())
            return null;

        if (!FetchControlCenterType())
            return null;

        if (!FetchControlCenterInstanceField())
            return null;

        if (!FetchCurrentControlledEnemyField())
            return null;

        var controlCenterInstance = GetControlCenterInstance();

        if (controlCenterInstance is null)
            return null;

        var currentControlledEnemy = _currentControlledEnemyField?.GetValue(controlCenterInstance);

        return currentControlledEnemy;
    }
}