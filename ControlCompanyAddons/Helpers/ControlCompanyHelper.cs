using System;
using System.Linq;
using System.Reflection;

namespace ControlCompanyAddons.Helpers;

public static class ControlCompanyHelper {
    private const string CONTROL_COMPANY_NAME = "ControlCompany, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
    private static Assembly? _controlCompanyAssembly;

    internal static bool FetchControlCompanyAssembly() {
        if (_controlCompanyAssembly is not null) return true;

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
            if (!assembly?.FullName?.Equals(CONTROL_COMPANY_NAME) ?? true) continue;

            _controlCompanyAssembly = assembly;
            return true;
        }

        ControlCompanyAddons.Logger.LogError("Couldn't find ControlCompany Assembly!");
        return false;
    }

    public static Assembly? GetControlCompanyAssembly() => !FetchControlCompanyAssembly()? null : _controlCompanyAssembly;
}