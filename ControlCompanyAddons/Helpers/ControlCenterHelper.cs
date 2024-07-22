using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace ControlCompanyAddons.Helpers;

public static class ControlCenterHelper {
    private static Type? _controlCenterType;
    private static FieldInfo? _controlCenterInstanceField;
    private static FieldInfo? _controlCenterCurrentModeField;
    private static FieldInfo? _currentControlledEnemyField;
    private static FieldInfo? _ghostControllerField;
    private static FieldInfo? _isGhostIndoorsField;
    private static MethodInfo? _enableGhostMethod;

    internal static bool FetchControlCenterType() {
        if (_controlCenterType is not null) return true;

        var controlCompanyAssembly = ControlCompanyHelper.GetControlCompanyAssembly();

        if (controlCompanyAssembly is null) return false;

        foreach (var type in AccessTools.GetTypesFromAssembly(controlCompanyAssembly)) {
            if (type?.Namespace?.Contains("ControlCompany.Core") is not true) continue;

            if (type?.Name?.Contains("ControlCenter") is not true) continue;

            _controlCenterType = type;
            return true;
        }

        ControlCompanyAddons.Logger.LogError("Couldn't find ControlCompany 'ControlCenter' type!");
        return false;
    }

    internal static bool FetchControlCenterInstanceField() {
        if (_controlCenterInstanceField is not null) return true;

        _controlCenterInstanceField = AccessTools.DeclaredField(_controlCenterType, "Instance");

        if (_controlCenterInstanceField is not null) return true;

        ControlCompanyAddons.Logger.LogError("Couldn't find ControlCompany 'ControlCenter' instance!");
        return false;
    }

    internal static bool FetchControlCenterCurrentModeField() {
        if (_controlCenterCurrentModeField is not null) return true;

        _controlCenterCurrentModeField = AccessTools.DeclaredField(_controlCenterType, "currentMode");

        if (_controlCenterCurrentModeField is not null) return true;

        ControlCompanyAddons.Logger.LogError("Couldn't find ControlCompany 'ControlCenter' currentMode field!");
        return false;
    }

    internal static bool FetchCurrentControlledEnemyField() {
        if (_currentControlledEnemyField is not null) return true;

        _currentControlledEnemyField = AccessTools.DeclaredField(_controlCenterType, "currentControlledEnemy");

        if (_currentControlledEnemyField is not null) return true;

        ControlCompanyAddons.Logger.LogError("Couldn't find ControlCompany 'ControlCenter' currentControlledEnemy field!");
        return false;
    }

    public static object? GetControlCenterInstance() {
        if (!ControlCompanyHelper.FetchControlCompanyAssembly()) return null;

        if (!FetchControlCenterType()) return null;

        if (!FetchControlCenterInstanceField()) return null;

        var controlCenterInstance = _controlCenterInstanceField?.GetValue(null);

        return controlCenterInstance;
    }

    public static void SetCurrentControlMode(ControlMode controlMode) {
        var controlCenterInstance = GetControlCenterInstance();

        if (controlCenterInstance is null) return;

        if (!FetchControlCenterCurrentModeField()) return;

        var enumType = _controlCenterType?.GetNestedType("Mode");

        if (enumType == null) {
            ControlCompanyAddons.Logger.LogError("Couldn't find ControlCompany 'ControlCenter.Mode' enum!");
            return;
        }

        var enumValues = Enum.GetValues(enumType);

        var enumToSet = enumValues.Cast<object?>()
                                  .FirstOrDefault(value => value is not null && value.ToString().Equals(controlMode.ToString()));

        if (enumToSet is null) {
            ControlCompanyAddons.Logger.LogError("Couldn't find ControlMode '" + controlMode + "' in ControlCompany!");
            return;
        }

        _controlCenterCurrentModeField?.SetValue(GetControlCenterInstance(), enumToSet);
    }

    public static ControlMode GetCurrentControlMode() {
        var controlCenterInstance = GetControlCenterInstance();

        if (controlCenterInstance is null) return ControlMode.UNKNOWN;

        if (!FetchControlCenterCurrentModeField()) return ControlMode.UNKNOWN;

        var currentMode = _controlCenterCurrentModeField?.GetValue(controlCenterInstance);

        if (currentMode is null) return ControlMode.UNKNOWN;

        var controlMode = currentMode.ToString() switch {
            "HOST" => ControlMode.HOST,
            "ENEMY" => ControlMode.ENEMY,
            "GHOST" => ControlMode.GHOST,
            var _ => ControlMode.UNKNOWN,
        };

        return controlMode;
    }

    public static Type? GetControlCenterType() {
        if (!ControlCompanyHelper.FetchControlCompanyAssembly()) return null;

        return !FetchControlCenterType()? null : _controlCenterType;
    }

    public static object? GetCurrentControlledEnemy() {
        if (!ControlCompanyHelper.FetchControlCompanyAssembly()) return null;

        if (!FetchControlCenterType()) return null;

        if (!FetchControlCenterInstanceField()) return null;

        if (!FetchCurrentControlledEnemyField()) return null;

        var controlCenterInstance = GetControlCenterInstance();

        if (controlCenterInstance is null) return null;

        var currentControlledEnemy = _currentControlledEnemyField?.GetValue(controlCenterInstance);

        return currentControlledEnemy;
    }

    internal static bool FetchEnableGhostMethod() {
        if (_enableGhostMethod is not null) return true;

        if (!FetchControlCenterType()) return false;

        _enableGhostMethod = AccessTools.DeclaredMethod(_controlCenterType, "EnableGhost", [
            typeof(bool), typeof(Vector3),
        ]);

        if (_controlCenterType is not null) return true;

        ControlCompanyAddons.Logger.LogError("Couldn't find ControlCompany 'ControlCenter' EnableGhost method!");
        return false;
    }

    public static void EnableGhost(bool enable, Vector3 position) {
        var controlCenterInstance = GetControlCenterInstance();

        if (controlCenterInstance is null) return;

        if (!FetchEnableGhostMethod()) return;

        _enableGhostMethod?.Invoke(controlCenterInstance, [
            enable, position,
        ]);
    }

    internal static bool FetchGhostControllerField() {
        if (_ghostControllerField is not null) return true;

        if (!FetchControlCenterType()) return false;

        var controlCenterType = GetControlCenterType();

        _ghostControllerField = AccessTools.DeclaredField(controlCenterType, "ghostController");

        if (_ghostControllerField is not null) return true;

        ControlCompanyAddons.Logger.LogError("Couldn't find ControlCompany 'ControlCenter' ghostController field!");
        return false;
    }

    public static object? GetGhostController() {
        if (!FetchControlCenterInstanceField()) return null;

        var controlCenterInstance = GetControlCenterInstance();

        if (controlCenterInstance is null) return null;

        if (!FetchGhostControllerField()) return null;

        var ghostController = _ghostControllerField?.GetValue(controlCenterInstance);
        return ghostController;
    }

    internal static bool FetchIsGhostIndoorsField() {
        if (!FetchControlCenterType()) return false;

        var controlCenterType = GetControlCenterType();

        if (controlCenterType is null) return false;

        _isGhostIndoorsField = AccessTools.DeclaredField(controlCenterType, "isGhostIndoors");

        if (_isGhostIndoorsField is not null) return true;

        ControlCompanyAddons.Logger.LogError("Couldn't find ControlCompany 'ControlCenter' isGhostIndoors field!");
        return false;
    }

    public static bool IsGhostIndoors() {
        if (!FetchControlCenterInstanceField()) return false;

        if (!FetchIsGhostIndoorsField()) return false;

        var controlCenterInstance = GetControlCenterInstance();

        if (controlCenterInstance is null) return false;

        var isGhostIndoors = _isGhostIndoorsField?.GetValue(controlCenterInstance);

        if (isGhostIndoors is null) return false;

        return (bool) isGhostIndoors;
    }
}