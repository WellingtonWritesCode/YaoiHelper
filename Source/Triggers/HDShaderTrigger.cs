using System.Collections.Generic;
using System.Linq;
using Celeste;
using Celeste.Mod;
using Celeste.Mod.Entities;
using Crackerberries.YaoiHelper.Types;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Crackerberries.YaoiHelper.Triggers;

[CustomEntity(["YaoiHelper/HDShader", $"{nameof(YaoiHelper)}/{nameof(HDShaderTrigger)}"])]
[Tracked]
public sealed class HDShaderTrigger : Trigger {
	public List<Shader> Shaders;
	private bool active;
	private readonly bool alwaysActive;
	private readonly string flagName;
	private readonly int priority;
	private readonly string? target;

	public bool Activated(Level level) {
		return active && flag(level);
	}

	private bool flag(Level level) {
		return flagName switch {
			"" => true,
			_ => level.Session.GetFlag(flagName),
		};
	}

	public HDShaderTrigger(EntityData data, Vector2 offset) : base(data, offset) {
		string[] textures = data.Attr("textures").Split(',').Select(x => x.Trim()).ToArray();
		priority = data.Int("priority");
		flagName = data.Attr("flag");
		target = data.Attr("target_register");
		alwaysActive = data.Bool("always_active");
		Shaders = data.Attr("effects").Split(',').Select(x => new Shader(new Effect(Engine.Graphics.GraphicsDevice, Everest.Content.Get($"Effects/{x.Trim()}.cso", true).Data), textures, string.IsNullOrEmpty(target) ? null : target, priority)).ToList();
	}

	public override void Awake(Scene scene) {
		base.Awake(scene);
		active = alwaysActive;
	}

	public override void OnEnter(Player player) {
		base.OnEnter(player);
		active = flag(player.level);
	}

	public override void OnLeave(Player player) {
		base.OnLeave(player);
		active = alwaysActive;
	}
}
