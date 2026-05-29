using System;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.YaoiHelper.Entities;

[CustomEntity("YaoiHelper/BuildController")]
[Tracked(false)]
public sealed class BuildController : Entity {
	public Grid origMap;
	private readonly int tileLimit;
	private readonly bool unlimited;

	public BuildController(EntityData data, Vector2 offset) : base() {
		tileLimit = data.Int("tile_limit");
		unlimited = data.Bool("unlimited");
	}

	public override void Awake(Scene scene) {
		base.Awake(scene);
		if (scene is not Level level) return;
		origMap = (Grid)level.SolidTiles.Grid.Clone();
		BuildHandler.TileLimit = tileLimit;
		BuildHandler.Unlimited = unlimited;
	}
}
