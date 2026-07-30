using Celeste;
using Celeste.Mod.Entities;
using Crackerberries.YaoiHelper.Handlers;
using Microsoft.Xna.Framework;
using Monocle;

namespace Crackerberries.YaoiHelper.Entities;

[CustomEntity($"{nameof(YaoiHelper)}/{nameof(BuildController)}")]
[Tracked(false)]
public sealed class BuildController : Entity {
	private readonly int tileLimit;
	private readonly bool unlimited;
	private readonly bool allowEntityMode;
	private readonly string flag;

	public BuildController(EntityData data, Vector2 offset) : base(data.Position + offset) {
		tileLimit = data.Int("tile_limit");
		unlimited = data.Bool("unlimited");
		allowEntityMode = data.Bool("allow_entity_mode");
		flag = data.String("flag");

	}

	public override void Awake(Scene scene) {
		base.Awake(scene);
		BuildHandler.TileLimit = tileLimit;
		BuildHandler.Unlimited = unlimited;
		BuildHandler.AllowEntityMode = allowEntityMode;
	}

    public override void Update() {
        base.Update();
		if (Scene is not Level level) return;
		BuildHandler.FlagSet = string.IsNullOrWhiteSpace(flag) || level.Session.GetFlag(flag);
    }
}
