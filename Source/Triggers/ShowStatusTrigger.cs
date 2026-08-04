using Celeste;
using Celeste.Mod;
using Celeste.Mod.Entities;
using Crackerberries.YaoiHelper.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Crackerberries.YaoiHelper.Triggers;

[CustomEntity($"{nameof(YaoiHelper)}/{nameof(ShowStatusTrigger)}")]
[Tracked]
internal sealed class ShowStatusTrigger : Trigger {
	private readonly string text;
	private readonly bool hideInCutscenes;
	private readonly float extraVPadding;
	private readonly bool instantRender;

	private StatusText? status;

	public ShowStatusTrigger(EntityData data, Vector2 offset) : base(data, offset) {
		text = data.Attr("text");
		hideInCutscenes = data.Bool("hideInCutscenes");
		extraVPadding = data.Float("extraVerticalPadding");
		instantRender = data.Bool("instantRender");
	}

	public override void OnEnter(Player player) {
		base.OnEnter(player);

		if (player is not null && player.level is not null) {
			if (status is not null) {
				// i don't think this should happen
				Logger.Log(LogLevel.Warn, $"{nameof(YaoiHelper)}/ShowStatusTrigger", "status already exists on enter..? replacing");
				status.RemoveSelf();
			}
			status = new StatusText(text, 0.7f, Color.White, extraVPad: extraVPadding);
			player.level.Add(status);

			if (instantRender) {
				player.level.OnEndOfFrame += () => {
					if (player is not null && player.level is not null)
						player.level.Entities.UpdateLists();
				};
			}
		}
	}

	public override void OnStay(Player player) {
		base.OnStay(player);

		if (hideInCutscenes)
			status?.ShouldRender = player.StateMachine.State != Player.StDummy;

		if (instantRender) {
			// for some reason, this is necessary to prevent it from sometimes
			// stopping to render 1 frame late
			player.level.OnEndOfFrame += () => {
				if (status is not null && (player is null || !CollideCheck(player))) {
					status.RemoveSelf();
					status = null;
					if (player is not null && player.level is not null)
						player.level.Entities.UpdateLists();
				}
			};
		}
	}

	public override void OnLeave(Player player) {
		base.OnLeave(player);

		status?.RemoveSelf();
		status = null;

		if (instantRender && player is not null && player.level is not null) {
			player.level.OnEndOfFrame += () => {
				if (player is not null && player.level is not null)
					player.level.Entities.UpdateLists();
			};
		}
	}

	public override void Removed(Scene scene) {
		status?.RemoveSelf();
		status = null;
	}
}
