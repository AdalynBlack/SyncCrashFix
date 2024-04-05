using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace SyncCrashFix.Patches;

[HarmonyPatch(typeof(StartOfRound))]
public class StartOfRoundPatches
{
	[HarmonyDebug]
	[HarmonyPatch("SyncShipUnlockablesServerRpc")]
	[HarmonyTranspiler]
	static IEnumerable<CodeInstruction> SyncUnlockablesCrashPatch(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
	{
		return new CodeMatcher(instructions, generator)
			.MatchForward(false,
					new CodeMatch(OpCodes.Blt))
			.Advance(-2)
			.CreateLabel(out var continueLabel)
			.MatchBack(false,
					new CodeMatch(OpCodes.Ldelem_Ref))
			.InsertAndAdvance(
					new CodeInstruction(OpCodes.Ldarg_0),
					new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(StartOfRound), "allPlayerScripts")),
					new CodeInstruction(OpCodes.Ldlen),
					new CodeInstruction(OpCodes.Bge_S, continueLabel),
					new CodeInstruction(OpCodes.Ldloc, 9))
			.InstructionEnumeration();
	}
}
