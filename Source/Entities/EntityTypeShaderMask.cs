using System;
using System.Collections.Generic;
using System.Linq;
using Celeste;
using Celeste.Mod.Entities;
using Celeste.Mod.Registry;
using Crackerberries.YaoiHelper.Interfaces;
using Microsoft.Xna.Framework;
using Monocle;

namespace Crackerberries.YaoiHelper.Entities;

[CustomEntity($"{nameof(YaoiHelper)}/{nameof(EntityTypeShaderMask)}")]
[Tracked]
public sealed class EntityTypeShaderMask : Entity, IShaderMask {
	private readonly List<string> sids;
	private readonly bool respectVisibility;
	private List<Entity?> entities = [];

	public List<string> MaskGroups { get; private set; }
    public bool LowRes { get; set; }

    public EntityTypeShaderMask(EntityData data, Vector2 offset) : base(data.Position + offset) {
		MaskGroups = data.Attr("mask_groups").Split(',').Select(x => x.Trim()).ToList();
		respectVisibility = data.Bool("respect_visibility");
		sids = data.Attr("entity_sids").Split(',').Select(x => x.Trim()).ToList();
		LowRes = data.Bool("low_res");
	}

    public override void Awake(Scene scene) {
		base.Awake(scene);
		if (scene is not Level level) return;
		entities = level.Entities.Where(x => x.SourceData is not null && sids.Contains(x.SourceData.Name) && x.SourceData.Level.Name == level.Session.Level).ToList();
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
			if (oldPos is Vector2 pos && entity is not null && (!respectVisibility || entity.Visible)) {
				entity.Position = Vector2.Transform(pos, level.Camera.Matrix);
				entity.Render();
				entity.Position = pos;
			}
		}
	}

    private void renderLowRes() {
		foreach (Entity? entity in entities) {
			if (entity is not null && (!respectVisibility || entity.Visible))
			entity.Render();
		}
    }
}
