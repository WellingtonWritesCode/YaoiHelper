using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;

using VivHelper;

namespace Celeste.Mod.YaoiHelper.Triggers;

[CustomEntity($"{nameof(YaoiHelper)}/{nameof(NonBrokenInstantCameraCatchupTrigger)}")]
internal sealed class NonBrokenInstantCameraCatchupTrigger : Trigger {
	private readonly string flag;
	private readonly bool flagInverted;

	public NonBrokenInstantCameraCatchupTrigger(EntityData data, Vector2 offset) : base(data, offset) {
		flag = data.Attr("flag");
		flagInverted = data.Bool("flagInverted");
	}

	public override void OnEnter(Player player) {
		base.OnEnter(player);

		if (string.IsNullOrEmpty(flag) || player.level.Session.GetFlag(flag) != flagInverted)
			VivHelperModule.Session.lockCamera = 1; // expire after 1f
	}
}
