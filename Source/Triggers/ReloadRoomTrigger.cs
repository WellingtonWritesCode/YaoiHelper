using Celeste;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Crackerberries.YaoiHelper.Triggers;

[CustomEntity($"{nameof(YaoiHelper)}/{nameof(ReloadRoomTrigger)}")]
public sealed class ReloadRoomTrigger : Trigger {
	private readonly string flag; 

    public ReloadRoomTrigger(EntityData data, Vector2 offset) : base(data, offset) {
		flag = data.Attr("flag");
    }

    public override void OnEnter(Player player) {
        base.OnEnter(player);

		if (string.IsNullOrEmpty(flag) || player.level.Session.GetFlag(flag)) {
			Engine.Scene = new LevelLoader(player.level.Session);
		}
    }
}
