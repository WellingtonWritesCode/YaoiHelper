using Celeste.Mod.YaoiHelper.Interop;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.YaoiHelper.Handlers;

[Submodule(HasSRTSupport = true)]
public static class GlobalTimerHandler {
	private static readonly List<GlobalFlagCountdown> countdowns = new();
	private static object countdownsSRTHandle;

	public static IReadOnlyList<GlobalFlagCountdown> Countdowns => countdowns;
	public static void ClearCountdowns() => countdowns.Clear();
	public static void AddCountdown(GlobalFlagCountdown c) => countdowns.Add(c);

	internal static void ApplyHooks() {
		On.Monocle.Engine.Update += On_EngineUpdate_TickCountdowns;
	}

	internal static void RemoveHooks() {
		On.Monocle.Engine.Update -= On_EngineUpdate_TickCountdowns;
	}

	internal static void RegisterSRTSupport() {
		countdownsSRTHandle = SpeedrunToolSaveLoadImports.RegisterStaticTypes(typeof(GlobalTimerHandler), [nameof(countdowns)]);
	}

	internal static void UnregisterSRTSupport() {
		SpeedrunToolSaveLoadImports.Unregister(countdownsSRTHandle);
		countdownsSRTHandle = null;
	}

	internal static void On_EngineUpdate_TickCountdowns(On.Monocle.Engine.orig_Update orig, Engine self, GameTime gameTime) {
		foreach (GlobalFlagCountdown countdown in countdowns) {
			// TODO: this breaks with assist mode slowdown or seeker/heart-collect slowdowns or such
			if (!paused(self.scene) || countdown.RunWhenPaused) {
				countdown.Current -= countdown.IgnoreFreezeFrames ? Engine.RawDeltaTime : Engine.DeltaTime;
			}
			if (countdown.Current <= 0) {
				countdown.Expired = !paused(self.scene) || countdown.RunWhenPaused;
			}
		}

		if (self.scene is Level level) {
			foreach (GlobalFlagCountdown countdown in countdowns.Where(x => x.Expired)) {
				level.Session.SetFlag(countdown.Flag, true);
			}

			countdowns.RemoveAll(x => x.Expired && (!paused(self.scene) || x.RunWhenPaused));
		}

		orig(self, gameTime);
	}

	private static bool paused(Scene scene) {
		if (SpeedrunTool.GetStateManagerState() == SpeedrunTool.StateManagerWaitingState)
			return true;
		return scene is Level { FrozenOrPaused: true } || scene.Paused;
	}
}

public sealed record GlobalFlagCountdown(string Flag, float Time, bool IgnoreFreezeFrames, bool RunWhenPaused) {
	public float Current = Time;
	public bool Expired = false;
}
