using System;
using System.Linq;
using System.Reflection;

namespace ControlCompanyAddons.Helpers;

public static class ControlCompanyHelper {
    private static Assembly? _controlCompanyAssembly;

    internal static bool FetchControlCompanyAssembly() {
        if (_controlCompanyAssembly is not null)
            return true;

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
            if (!assembly?.FullName?.ToLower().Contains("controlcompany") ?? true)
                continue;

            if (assembly?.FullName?.ToLower().Contains("controlcompanyfilter") ?? true)
                continue;

            _controlCompanyAssembly = assembly;
            return true;
        }

        ControlCompanyAddons.Logger.LogError("Couldn't find ControlCompany Assembly!");
        return false;
    }

    public static Assembly? GetControlCompanyAssembly() =>
        !FetchControlCompanyAssembly()? null : _controlCompanyAssembly;
}