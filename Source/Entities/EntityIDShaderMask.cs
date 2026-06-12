using System;
using System.Collections.Generic;
using System.Linq;
using Celeste.Mod.Entities;
using Celeste.Mod.YaoiHelper.Interfaces;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.YaoiHelper.Entities;

[CustomEntity($"{nameof(YaoiHelper)}/{nameof(EntityIDShaderMask)}")]
[Tracked]
public sealed class EntityIDShaderMask : Entity, IShaderMask {
	private readonly List<string> groups;
	private readonly List<int> ids;
	private List<Entity?> entities = [];

	public List<string> MaskGroups => groups;

	public EntityIDShaderMask(EntityData data, Vector2 offset) : base(data.Position + offset) {
		groups = data.Attr("mask_groups").Split(',').Select(x => x.Trim()).ToList();
		ids = data.Attr("entity_ids").Split(',').Select(x => int.Parse(x.Trim())).ToList();
	}

    public override void Awake(Scene scene) {
		base.Awake(scene);
		if (scene is not Level level) return;
		entities = ids.Select(i => level.Entities.FirstOrDefault(x => x.SourceId.ID == i && x.SourceId.Level == SourceId.Level)).ToList();
		
    }

	public void RenderMask() {
		foreach (Entity? entity in entities) {
			Vector2? oldPos = entity?.Position;
			if (oldPos is Vector2 pos) {
				entity?.Position = Vector2.Transform(pos, SceneAs<Level>().Camera.Matrix);
				entity?.Render();
				// entity?.Components.Render();
				entity?.Position = pos;
			}
		}
	}
}
