using System.Collections.Generic;
using System.Linq;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.YaoiHelper.Triggers;

// a bunch of this is lifted from frosthelper
[CustomEntity($"{nameof(YaoiHelper)}/{nameof(GlobalTimer)}")]
[Tracked]
public sealed class GlobalTimer(EntityData data, Vector2 offset) : Trigger(data, offset) {
	private readonly string flag = data.Attr("flag");
	private readonly float time = data.Float("time");
	private readonly bool ignoreFreezeFrames = data.Bool("ignore_freeze_frames");
	private readonly bool runWhenPaused = data.Bool("run_when_paused");

	public override void Awake(Scene scene) {
		base.Awake(scene);
		GlobalTimerHandler.countdowns = [];
	}

	public override void OnEnter(Player player) {
		base.OnEnter(player);
		GlobalTimerHandler.countdowns.Add(new GlobalFlagCountdown(flag, time, ignoreFreezeFrames, runWhenPaused));
		player.level.Session.SetFlag(flag, false);

	}

	public override void DebugRender(Camera camera) {
		base.DebugRender(camera);
		ActiveFont.Draw(string.Join('\n', GlobalTimerHandler.countdowns.Select(x => string.Concat(x.Flag, " : ", x.Current))), camera.Position, Color.Red);
	}
}

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
