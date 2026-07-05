using System;
using System.Collections.Generic;
using System.Linq;
using Celeste.Mod.Entities;
using Celeste.Mod.Registry;
using Celeste.Mod.YaoiHelper.Interfaces;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.YaoiHelper.Entities;

[CustomEntity($"{nameof(YaoiHelper)}/{nameof(EntityTypeShaderMask)}")]
[Tracked]
public sealed class EntityTypeShaderMask : Entity, IShaderMask {
	private readonly List<string> sids;
	private List<Entity?> entities = [];

	public List<string> MaskGroups { get; private set; }
    public bool LowRes { get; set; }

    public EntityTypeShaderMask(EntityData data, Vector2 offset) : base(data.Position + offset) {
		MaskGroups = data.Attr("mask_groups").Split(',').Select(x => x.Trim()).ToList();
		sids = data.Attr("entity_sids").Split(',').Select(x => x.Trim()).ToList();
		LowRes = data.Bool("low_res");
	}

    public override void Awake(Scene scene) {
		base.Awake(scene);
		if (scene is not Level level) return;
		List<Type> types = sids.SelectMany(x => EntityRegistry.GetKnownTypesFromSid(x)).ToList();
		entities = scene.Entities.Where(x => types.Contains(x.GetType()) && x.SourceData.Level.Name == level.Session.Level).ToList();
    }

	public void RenderMask() {
		if (LowRes) {
			renderLowRes();
		} else {
			renderHiRes();
		}
	}

	private void renderHiRes() {
		if (Scene is not Level level) return;
		foreach (Entity? entity in entities) {
			Vector2? oldPos = entity?.Position;
			if (oldPos is Vector2 pos) {
				entity?.Position = Vector2.Transform(pos, level.Camera.Matrix);
				entity?.Render();
				entity?.Position = pos;
			}
		}
	}

    private void renderLowRes() {
		foreach (Entity? entity in entities) {
			entity?.Render();
		}
    }
}
