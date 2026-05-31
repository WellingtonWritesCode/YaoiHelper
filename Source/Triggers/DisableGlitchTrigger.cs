using System.Linq;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.RuntimeDetour;

namespace Celeste.Mod.YaoiHelper.Triggers;

[Submodule]
[CustomEntity($"{nameof(YaoiHelper)}/{nameof(DisableGlitchTrigger)}")]
[Tracked]
public sealed class DisableGlitchTrigger : Trigger {
	public bool AlwaysActive { get; }
	public bool Activated { get; private set; }

	public DisableGlitchTrigger(EntityData data, Vector2 offset) : base(data, offset) {
		AlwaysActive = data.Bool("always_active");
		Activated = AlwaysActive;
	}

	public override void OnEnter(Player player) {
		base.OnEnter(player);
		Activated = true;
	}

	public override void OnLeave(Player player) {
		base.OnLeave(player);
		Activated = false || AlwaysActive;
	}

	internal static void ApplyHooks() {
		using (new DetourConfigContext(new DetourConfig(
			$"{YaoiHelperModule.DefaultDetourID}_{nameof(DisableGlitchTrigger)}",
			priority: int.MinValue // hook as late as possible as to not intervene with other people's stuff
		)).Use()) {
			On.Celeste.Glitch.Apply += On_GlitchApply_DisableIfTrigger;
		}
	}

	internal static void RemoveHooks() {
		On.Celeste.Glitch.Apply -= On_GlitchApply_DisableIfTrigger;
	}

	internal static void On_GlitchApply_DisableIfTrigger(On.Celeste.Glitch.orig_Apply orig, VirtualRenderTarget target, float timer, float seed, float amplitude) {
		if (!Engine.Scene.Tracker.GetEntities<DisableGlitchTrigger>().Cast<DisableGlitchTrigger>().Any(x => x.Activated)) {
			orig(target, timer, seed, amplitude);
		}
	}
}
