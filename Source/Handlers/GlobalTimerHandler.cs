using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.YaoiHelper.Handlers;

public static class GlobalTimerHandler {
	public static List<GlobalFlagCountdown> countdowns = new List<GlobalFlagCountdown>();

	public static void On_EngineUpdate_TickCountdowns(On.Monocle.Engine.orig_Update orig, Engine self, GameTime gameTime) {
		foreach (GlobalFlagCountdown countdown in countdowns) {
			// ternary operator cow: eat my tergers
			countdown.Current -= (countdown.ignoreFreezeFrames ? Engine.RawDeltaTime : Engine.DeltaTime) * (!self.scene.Paused || countdown.runWhenPaused ? 1 : 0);
			if (countdown.Current <= 0) {
				countdown.Expired = !self.scene.Paused || countdown.runWhenPaused;
			}
		}

		if (self.scene is Level level) {
			foreach (GlobalFlagCountdown countdown in countdowns.Where(x => x.Expired)) {
				level.Session.SetFlag(countdown.Flag, true);
			}

			countdowns.RemoveAll(x => x.Expired && (!self.scene.Paused || x.runWhenPaused));
		}


		orig(self, gameTime);
	}

	public static void ApplyHooks() {
		On.Monocle.Engine.Update += On_EngineUpdate_TickCountdowns;
	}

	public static void RemoveHooks() {
		On.Monocle.Engine.Update -= On_EngineUpdate_TickCountdowns;

	}
}

public record GlobalFlagCountdown(string Flag, float Time, bool ignoreFreezeFrames, bool runWhenPaused) { 
	public float Current = Time;
	public bool Expired = false;
}
