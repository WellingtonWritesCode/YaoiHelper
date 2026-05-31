using System;
using System.Linq;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Cil;

namespace Celeste.Mod.YaoiHelper.Triggers;

[Submodule]
[CustomEntity(["YaoiHelper/PlayerOpacity", $"{nameof(YaoiHelper)}/{nameof(PlayerOpacityTrigger)}"])]
public sealed class PlayerOpacityTrigger : Trigger {
	// TODO: make this a data component
	public static float Opacity = 1f; // TODO kill everyone
	public readonly float setOpacity;

	public PlayerOpacityTrigger(EntityData data, Vector2 offset) : base(data, offset) {
		setOpacity = data.Float("opacity");
	}

	public override void OnEnter(Player player) {
		Opacity = setOpacity;
	}

	private static float colorFactor(Image image) {
		if (Engine.Scene is not Level level) return 1f;
		if (level.Tracker.GetEntity<Player>() is not Player player) return 1f;
		if (!player.Components.Select(x => image.GetType() == x.GetType()).Any(x => x == true)) return 1f;

		player.Hair.Alpha = Opacity;
		return Opacity;
	}

	public static void IL_ImageRender_SetPlayerOpacity(ILContext il) {
		ILCursor cursor = new ILCursor(il);

		cursor.GotoNext(MoveType.After, cursor => cursor.MatchLdfld(typeof(GraphicsComponent).GetField("Color")));
		cursor.EmitLdarg0();
		cursor.EmitDelegate(colorFactor);
		cursor.EmitCall(typeof(Color).GetMethod("op_Multiply"));
	}

	public static void ApplyHooks() {
		IL.Monocle.Image.Render += IL_ImageRender_SetPlayerOpacity;
	}

	public static void RemoveHooks() {
		IL.Monocle.Image.Render -= IL_ImageRender_SetPlayerOpacity;
	}
}
