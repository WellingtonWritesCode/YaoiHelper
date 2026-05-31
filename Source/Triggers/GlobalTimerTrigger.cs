using System.Linq;
using Celeste.Mod.Entities;
using Celeste.Mod.YaoiHelper.Handlers;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.YaoiHelper.Triggers;

// a bunch of this is lifted from frosthelper
[CustomEntity(["YaoiHelper/GlobalTimer", $"{nameof(YaoiHelper)}/{nameof(GlobalTimerTrigger)}"])]
[Tracked]
public sealed class GlobalTimerTrigger(EntityData data, Vector2 offset) : Trigger(data, offset) {
	private readonly string flag = data.Attr("flag");
	private readonly float time = data.Float("time");
	private readonly bool ignoreFreezeFrames = data.Bool("ignore_freeze_frames");
	private readonly bool runWhenPaused = data.Bool("run_when_paused");

	public override void Awake(Scene scene) {
		base.Awake(scene);
		GlobalTimerHandler.ClearCountdowns();
	}

	public override void OnEnter(Player player) {
		base.OnEnter(player);
		GlobalTimerHandler.AddCountdown(new GlobalFlagCountdown(flag, time, ignoreFreezeFrames, runWhenPaused));
		player.level.Session.SetFlag(flag, false);
	}

	public override void DebugRender(Camera camera) {
		base.DebugRender(camera);
		ActiveFont.Draw(string.Join('\n', GlobalTimerHandler.Countdowns.Select(x => string.Concat(x.Flag, " : ", x.Current))), new Vector2(camera.Position.X, camera.Position.Y + camera.Viewport.Height / 2), Vector2.Zero, Vector2.One / 3, Color.Red);
	}
}
