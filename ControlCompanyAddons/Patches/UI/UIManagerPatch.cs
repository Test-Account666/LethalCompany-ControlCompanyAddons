using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using ControlCompany.Core.UI;
using ControlCompanyAddons.Helpers;
using HarmonyLib;

namespace ControlCompanyAddons.Patches.UI;

[HarmonyPatch(typeof(UIManager))]
public static class UIManagerPatch {
    private static readonly Regex _DirtyKeyRegex1 = new(".*<.*>/", RegexOptions.Compiled);
    private static readonly Regex _DirtyKeyRegex2 = new(@"\[.*\]", RegexOptions.Compiled);
    private static string _keyToPress = "";
    private static long _nextUpdate = 0;

    [HarmonyPatch(nameof(UIManager.RenderEnemyControls))]
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> InsertEnemyControls(IEnumerable<CodeInstruction> instructions) {
        List<CodeInstruction> codes = [
            ..instructions,
        ];
        var found = false;

        // The pattern of IL instructions to match
        // Pattern is in strings, because I'm stupid or something
        List<string> pattern = [
            "ldloc.2 NULL [Label2]", "newobj void ControlCompany.Core.UI.UIInstruction::.ctor()", "dup NULL", "ldarg.s 4",
            "callvirt void ControlCompany.Core.UI.UIInstruction::set_InputKey(string value)", "nop NULL", "dup NULL", "ldstr \"Exit\"",
            "callvirt void ControlCompany.Core.UI.UIInstruction::set_ActionText(string value)", "nop NULL",
            "callvirt virtual void System.Collections.Generic.List<ControlCompany.Core.UI.UIInstruction>::Add(ControlCompany.Core.UI.UIInstruction item)",
            "nop NULL",
        ];

        /*
            IL_00c4: ldloc.2      // playerEnemyControls
            IL_00c5: newobj       instance void ControlCompany.Core.UI.UIInstruction::.ctor()
            IL_00ca: dup
            IL_00cb: ldarg.s      exitKey
            IL_00cd: callvirt     instance void ControlCompany.Core.UI.UIInstruction::set_InputKey(string)
            IL_00d2: nop
            IL_00d3: dup
            IL_00d4: ldstr        "Exit"
            IL_00d9: callvirt     instance void ControlCompany.Core.UI.UIInstruction::set_ActionText(string)
            IL_00de: nop
            IL_00df: callvirt     instance void class [mscorlib]System.Collections.Generic.List`1<class ControlCompany.Core.UI.UIInstruction>::Add(!0/*class ControlCompany.Core.UI.UIInstruction* /)
            IL_00e4: nop
        */

        var patternIndex = 0;

        for (var index = 0; index < codes.Count; index++) {
            var codeInstruction = codes[index];
            var codeInstructionToMatch = pattern[patternIndex];

            if (codeInstruction.ToString().Equals(codeInstructionToMatch))
                // Yay! a match! Let's go to the next instruction 
                patternIndex += 1;
            else
                // No :( Back to start
                patternIndex = 0;

            // If we're not finished matching, let's go back to the start of the loop :)
            if (patternIndex < pattern.Count) continue;

            // Yay, we fully matched our pattern :>
            found = true;

            var getInputKeyMethod = AccessTools.DeclaredMethod(typeof(UIManagerPatch), nameof(GetInputKey));

            // Insert the new code right after the original code
            codes.InsertRange(index + 1, [
                // Load `playerEnemyControls` variable
                new(OpCodes.Ldloc_2),
                // Create a new UIInstruction instance
                new(OpCodes.Newobj, typeof(UIInstruction).GetConstructor(Type.EmptyTypes)),
                // Duplicate the top operand stack item
                new(OpCodes.Dup),
                // Load input key string onto the stack
                //new(OpCodes.Ldstr, GetInputKey()),

                // Call GetInputKey method to get the input key string
                new(OpCodes.Call, getInputKeyMethod),

                // Set the InputKey property
                new(OpCodes.Callvirt,
                    typeof(UIInstruction).GetProperty("InputKey", BindingFlags.Public | BindingFlags.Instance)?.SetMethod),
                // No operation
                new(OpCodes.Nop),
                // Duplicate the top operand stack item
                new(OpCodes.Dup),
                // Load string "Release Control" onto the stack
                new(OpCodes.Ldstr, "Release Control"),
                // Set the ActionText property
                new(OpCodes.Callvirt,
                    typeof(UIInstruction).GetProperty("ActionText", BindingFlags.Public | BindingFlags.Instance)?.SetMethod),
                // No operation
                new(OpCodes.Nop),
                // Add the UIInstruction instance to `playerEnemyControls` list
                new(OpCodes.Callvirt, AccessTools.DeclaredMethod(typeof(List<UIInstruction>), "Add")),
                // No operation
                new(OpCodes.Nop),
            ]);

            // Exit the loop since we've inserted the new code
            break;
        }

        if (!found)
            throw new ArgumentException("Unable to locate the correct position to insert the new code for rendering the release prompt.");

        return codes;
    }

    private static string GetInputKey() {
        var currentTime = UnixTime.GetCurrentTime();

        if (currentTime < _nextUpdate) return _keyToPress;

        var keyToPress = ControlCompanyAddons.inputKeys.ReleaseControlKey.bindings[0].ToString().ToUpper();
        keyToPress = _DirtyKeyRegex1.Replace(keyToPress, "");

        keyToPress = _DirtyKeyRegex2.Replace(keyToPress, "");

        _keyToPress = keyToPress;

        //_nextUpdate = currentTime + 5000;

        return keyToPress;
    }
}