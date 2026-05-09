using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.YaoiHelper.Triggers;

// a bunch of this is lifted from frosthelper
[CustomEntity($"{nameof(YaoiHelper)}/{nameof(GlobalTimer)}")]
[Tracked]
public sealed class GlobalTimer : Trigger {
	private readonly string flag;
	private readonly float time;
	internal float current;
	internal bool started;

	public GlobalTimer(EntityData data, Vector2 offset) : base(data, offset) {
		flag = data.Attr("flag");
		current = time = data.Float("time");
	}

	public override void Added(Scene scene) {
		base.Added(scene);

		if (scene is Level level) {
			level.Session.SetFlag(flag, false);
		}
	}

	public override void OnEnter(Player player) {
		base.OnEnter(player);

		started = true;
	}

	public override void Update() {
		base.Update();

		if (!started) return;
		current -= Engine.RawDeltaTime;
		if (current <= 0) {
			SceneAs<Level>().Session.SetFlag(flag, true);
			RemoveSelf();
		}
	}

	public void Reset() {
		started = false;
		current = time;
	}
}
