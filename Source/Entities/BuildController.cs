using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.YaoiHelper.Entities;

[CustomEntity("YaoiHelper/BuildController")]
[Tracked(false)]
public sealed class BuildController : Entity {
	public BuildController(EntityData data, Vector2 offset) : base() {
	}
}
