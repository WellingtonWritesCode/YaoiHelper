using System.Collections.Generic;
using System.Linq;
using Celeste;
using Celeste.Mod;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Crackerberries.YaoiHelper.Entities;

// TODO: maybe use level.OnEndOfFrame after looking into potential perf implications
[CustomEntity($"{nameof(YaoiHelper)}/{nameof(EntityInvisibilityField)}")]
[Tracked]
[Submodule]
public sealed class EntityInvisibilityField : Entity {
	public List<Entity> Entities = [];

	internal static void ApplyHooks() {
		Everest.Events.Level.OnAfterUpdate += onAfterUpdate_MakeEntitiesInvisible;
	}

	internal static void RemoveHooks() {
		Everest.Events.Level.OnAfterUpdate -= onAfterUpdate_MakeEntitiesInvisible;
	}
	
	private static void onAfterUpdate_MakeEntitiesInvisible(Level level) {
		foreach (Entity entity in level.Tracker.GetEntities<EntityInvisibilityField>().Cast<EntityInvisibilityField>().SelectMany(x => x.Entities).Distinct()) {
			entity.Visible = false;
		}
	}
	
    public EntityInvisibilityField(EntityData data, Vector2 offset) : base(data.Position + offset) {
		Collider = new Hitbox(data.Width, data.Height);
		Visible = false;
	}

    public override void Awake(Scene scene) {
		base.Awake(scene);
		if (scene is not Level level) return;
		Entities = level.Entities.Where(x => x.Collider is not null && Collider.Collide(x)).ToList();
    }

}
