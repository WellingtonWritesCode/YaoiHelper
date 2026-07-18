using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.YaoiHelper.Triggers;

[CustomEntity($"{nameof(YaoiHelper)}/{nameof(ReloadRoomTrigger)}")]
public sealed class ReloadRoomTrigger : Trigger {
    public ReloadRoomTrigger(EntityData data, Vector2 offset) : base(data, offset) {
    }

    public override void OnEnter(Player player) {
        base.OnEnter(player);
		Engine.Scene = new LevelLoader(player.level.Session);
    }
}
