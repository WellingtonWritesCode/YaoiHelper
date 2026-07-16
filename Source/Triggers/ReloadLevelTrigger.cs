using System.Linq;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.YaoiHelper.Triggers;

[CustomEntity($"{nameof(YaoiHelper)}/{nameof(ReloadLevelTrigger)}")]
public sealed class ReloadLevelTrigger : Trigger {
    public ReloadLevelTrigger(EntityData data, Vector2 offset) : base(data, offset) {
    }

    public override void Awake(Scene scene) {
        base.Awake(scene);
		if (scene is not Level level) return;
		foreach (Player player in level.Tracker.GetEntities<Player>().Cast<Player>()) {
			if (player.Collider.Collide(Collider)) {
				RemoveSelf();
			}
		}
    }

    public override void OnEnter(Player player) {
        base.OnEnter(player);
		AssetReloadHelper.ReloadLevel();
    }
}
