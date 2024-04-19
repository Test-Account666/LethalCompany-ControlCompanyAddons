using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using ControlCompanyAddons.Helpers;
using HarmonyLib;

namespace ControlCompanyAddons.Patches.Controller;

[HarmonyPatch]
public static class RadMechEnemyControllerPatch {
    [HarmonyPatch(MethodType.Enumerator)]
    public static MethodBase TargetMethod() {
        var controlCompanyAssembly = ControlCompanyHelper.GetControlCompanyAssembly();

        if (controlCompanyAssembly is null)
            throw new NullReferenceException("ControlCompany Assembly could not be found!");

        var radMechControllerEnumeratorType =
            AccessTools.TypeByName("ControlCompany.Core.Enemy.RadMechEnemyController+<DelayedDestroyAndCleanUp>d__15")
         ?? AccessTools
            .GetTypesFromAssembly(controlCompanyAssembly)
            .Where(type => type?.Namespace?.Contains("ControlCompany.Core.Enemy") is true)
            .FirstOrDefault(type => type?.Name?.Contains("RadMechEnemyController+<DelayedDestroyAndCleanUp>d__15") is true);

        if (radMechControllerEnumeratorType is null)
            throw new NullReferenceException("Could not find ControlCompany DelayedDestroyAndCleanUp type!");

        var renderEnemyControlsMethod = AccessTools.DeclaredMethod(radMechControllerEnumeratorType, "MoveNext");

        if (renderEnemyControlsMethod is null)
            throw new NullReferenceException("ControlCompany 'RadMechEnemyController' MoveNext method could not be found!");

        return renderEnemyControlsMethod;
    }

    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
        List<CodeInstruction> codes = [
            ..instructions,
        ];
        var found = false;


        /*IL_003a: ldarg.0      // this
        IL_003b: ldc.r4       3
        IL_0040: newobj       instance void [UnityEngine.CoreModule]UnityEngine.WaitForSeconds::.ctor(float32)
        IL_0045: stfld        object ControlCompany.Core.Enemy.RadMechEnemyController/'<DelayedDestroyAndCleanUp>d__15'::'<>2__current'*/

        for (var index = 0; index < codes.Count - 3; index++) {
            var codeInstruction1 = codes[index];
            var codeInstruction2 = codes[index + 1];
            var codeInstruction3 = codes[index + 2];
            var codeInstruction4 = codes[index + 3];

            if (codeInstruction1.opcode != OpCodes.Ldarg_0)
                continue;

            if (codeInstruction2.opcode != OpCodes.Ldc_R4 && codeInstruction2.operand is not 3)
                continue;

            if (codeInstruction3.opcode != OpCodes.Newobj)
                continue;

            if (codeInstruction4.opcode != OpCodes.Stfld)
                continue;

            // Replace `new WaitForSeconds(3)` with `new WaitForSeconds(0)` to remove waiting time
            codes[index + 1] = new(OpCodes.Ldc_R4, .0F);
            found = true;
        }

        if (!found)
            throw new ArgumentException("Couldn't find correct code instructions!");

        return codes;
    }
}