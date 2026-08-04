using System.Linq;
using Celeste;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Crackerberries.YaoiHelper.Triggers;

// a bunch of this is referenced from crystalline
[CustomEntity($"{nameof(YaoiHelper)}/{nameof(SpeedFadeTrigger)}")]
[Tracked(false)]
[Submodule]
public sealed class SpeedFadeTrigger : Trigger {
	private readonly Vector2[] nodes;
	
	public Trigger? Trigger;
	public readonly float LowerBound;
	public readonly float UpperBound;
	public readonly bool XOnly;
	public readonly bool YOnly;

    public SpeedFadeTrigger(EntityData data, Vector2 offset) : base(data, offset) {
		nodes = data.NodesOffset(offset);
		LowerBound = data.Float("lower_bound");
		UpperBound = data.Float("upper_bound");
		XOnly = data.Bool("x_only");
		YOnly = data.Bool("y_only");
		
    }

	internal static void ApplyHooks() {
		On.Celeste.Trigger.GetPositionLerp += on_TriggerGetPositionLerp_ApplySpeedFade;
	}

	internal static void RemoveHooks() {
		On.Celeste.Trigger.GetPositionLerp -= on_TriggerGetPositionLerp_ApplySpeedFade;
	}

	private static float on_TriggerGetPositionLerp_ApplySpeedFade(On.Celeste.Trigger.orig_GetPositionLerp orig, Trigger self, Player player, PositionModes mode) {
		if (self.Scene is not Level level) return orig(self, player, mode);
		foreach (SpeedFadeTrigger speedFadeTrigger in level.Tracker.GetEntities<SpeedFadeTrigger>().Cast<SpeedFadeTrigger>()) {
			if (speedFadeTrigger.Trigger == self) {
				return float.Clamp((((speedFadeTrigger.XOnly || speedFadeTrigger.YOnly) ? (speedFadeTrigger.XOnly ? player.Speed.X : player.Speed.Y) : player.Speed.Length()) - speedFadeTrigger.LowerBound) / (speedFadeTrigger.UpperBound - speedFadeTrigger.LowerBound), 0f, 1f); 
			}
		}	

		return orig(self, player, mode);
	}

    public override void Awake(Scene scene) {
        base.Awake(scene);
		Trigger = scene.CollideFirst<Trigger>(nodes[0]) ?? scene.Tracker.GetNearestEntity<Trigger>(nodes[0]);
		Trigger?.Collidable = false;
    }

    public override void OnEnter(Player player) {
        base.OnEnter(player);
		Trigger?.OnEnter(player);
    }

    public override void OnStay(Player player) {
        base.OnStay(player);
		Trigger?.OnStay(player);
    }

    public override void OnLeave(Player player) {
        base.OnLeave(player);
		Trigger?.OnLeave(player);
    }

}
