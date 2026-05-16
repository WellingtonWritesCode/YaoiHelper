using System.Collections.Generic;
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

	public override void Awake(Scene scene) {
		base.Awake(scene);
		GlobalTimerHandler.countdowns = [];
	}

	public override void OnEnter(Player player) {
		base.OnEnter(player);
		GlobalTimerHandler.countdowns.Add(new GlobalFlagCountdown(flag, time));
		player.level.Session.SetFlag(flag, false);

	}
}

public static class GlobalTimerHandler {
	public static List<GlobalFlagCountdown> countdowns = new List<GlobalFlagCountdown>();

	public static void OnLevelUpdate_TickCountdowns(On.Celeste.Level.orig_Update orig, Level self) {
		foreach (GlobalFlagCountdown countdown in countdowns) {
			countdown.Current -= Engine.RawDeltaTime;
			if (countdown.Current <= 0) {
				self.Session.SetFlag(countdown.Flag, true);
			}
		}

		countdowns.RemoveAll(x => x.Current <= 0);

		orig(self);
	}

	public static void ApplyHooks() {
		On.Celeste.Level.Update += OnLevelUpdate_TickCountdowns;
	}

	public static void RemoveHooks() {
		On.Celeste.Level.Update -= OnLevelUpdate_TickCountdowns;

	}
}

public record GlobalFlagCountdown(string Flag, float Time) { 
	public float Current = Time;
}
