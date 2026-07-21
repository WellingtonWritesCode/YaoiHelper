using System.Linq;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.YaoiHelper.Triggers;

[CustomEntity($"{nameof(YaoiHelper)}/{nameof(HotReloadRoomTrigger)}")]
public sealed class HotReloadRoomTrigger : Trigger {
	private readonly string flag;

    public HotReloadRoomTrigger(EntityData data, Vector2 offset) : base(data, offset) {
		flag = data.Attr("flag");
    }

    public override void Awake(Scene scene) {
        base.Awake(scene);
		if (scene is not Level level) return;
		if (level.Session.GetFlag($"hot-reload-room-trigger-{SourceData.Level}:{SourceId.ID}")) {
			level.Session.SetFlag($"hot-reload-room-trigger-{SourceData.Level}:{SourceId.ID}", false);
			RemoveSelf();
		}
    }

    public override void OnEnter(Player player) {
        base.OnEnter(player);
		if (string.IsNullOrEmpty(flag) || player.level.Session.GetFlag(flag)) {
			player.level.Session.SetFlag($"hot-reload-room-trigger-{SourceData.Level}:{SourceId.ID}", true);
			AssetReloadHelper.ReloadLevel();
		}
    }
}
