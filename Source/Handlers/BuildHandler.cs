using System;
using System.Linq;
using Celeste.Mod.Entities;
using Celeste.Mod.YaoiHelper.Triggers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Monocle;
using MonoMod.Cil;

namespace Celeste.Mod.YaoiHelper.Entities;

public static class BuildHandler {
	public static Vector2 mouse_pos;
	public static bool building;
	public static bool mining;
	public static bool valid_position;

	private static readonly char selectedTile = '3';
	
	private static int tileLimit, tilesLeft = -1;
	private static bool unlimited = true;

	public static int TileLimit { get => tileLimit; set => tileLimit = tilesLeft = value; }
	public static int TilesLeft { get => tilesLeft; private set => tilesLeft = value; }
	public static bool Unlimited { get => unlimited || YaoiHelperModule.Settings.BuildAnywhere; set => unlimited = value; }

	public static void On_LevelUpdate_Build(On.Celeste.Level.orig_Update orig, Level level) {
		orig(level);

		if (level.FrozenOrPaused || (level.Tracker.CountEntities<BuildController>() == 0 && !YaoiHelperModule.Settings.BuildAnywhere)) return; 

		if (YaoiHelperModule.Settings.BuildAnywhere) {
			unlimited = true;
		}

		BuildController controller = level.Tracker.GetEntity<BuildController>();

		MouseState state = MInput.Mouse.CurrentState;
		mouse_pos = level.ScreenToWorld(new Vector2(MInput.Mouse.X - Engine.Viewport.X, MInput.Mouse.Y - Engine.Viewport.Y)) - level.LevelOffset;
		Point tile = new Point((int)mouse_pos.X / 8, (int)mouse_pos.Y / 8) + level.LevelSolidOffset;

		building = state.LeftButton.HasFlag(ButtonState.Pressed);
		mining = state.RightButton.HasFlag(ButtonState.Pressed);


		if (level.Tracker.CountEntities<BuildRegion>() == 0) {
			valid_position = true;
		} else {
			valid_position = false;
			foreach (BuildRegion buildRegion in level.Tracker.GetEntities<BuildRegion>().Cast<BuildRegion>()) {
				valid_position = valid_position || (buildRegion.Collider as Hitbox).Collide(mouse_pos + level.LevelOffset);
			}
		}

		if (!(building || mining) || !valid_position) return;

		if (building) {
			if (level.SolidsData[tile.X, tile.Y] == '0' && ((tilesLeft > 0) || Unlimited))  {
				level.SolidTiles.Grid[tile.X, tile.Y] = true;
				level.SolidsData[tile.X, tile.Y] = selectedTile;
				UpdateTilesAround(level, tile, 2);

				if (tilesLeft > 0 && !Unlimited) {
					tilesLeft--;
				}
			}
		} else { // mining
			if (level.SolidsData[tile.X, tile.Y] != '0') {
				level.SolidTiles.Grid[tile.X, tile.Y] = false;
				level.SolidsData[tile.X, tile.Y] = '0';
				UpdateTilesAround(level, tile, 2);

				if (!Unlimited && controller is not null && (!controller.origMap[tile.X, tile.Y])) {
					tilesLeft++;
				}
			}
		}
	}

	private static void UpdateTilesAround(Level level, Point tile, int radius) {
		Autotiler.Generated genned = GFX.FGAutotiler.Generate(level.SolidsData, tile.X - radius, tile.Y - radius, 2 * radius + 1, 2 * radius + 1, forceSolid: false, '0', new Autotiler.Behaviour {
			EdgesExtend = true,
			EdgesIgnoreOutOfLevel = false,
			PaddingIgnoreOutOfLevel = false
		});

		for (int i = -radius; i <= radius; i++) {
			for (int j = -radius; j <= radius; j++) {
				level.SolidTiles.Tiles.Tiles[tile.X + i, tile.Y + j] = genned.TileGrid.Tiles[i + radius, j + radius];
			}
		}
	}

	public static void OnLoadingThread_AddCursorDisplay(Level level) {
		level.Add(new BuildCursorDisplay());
	}

	public static void ApplyHooks() {
		On.Celeste.Level.Update += On_LevelUpdate_Build;
		Everest.Events.LevelLoader.OnLoadingThread += OnLoadingThread_AddCursorDisplay;
	}

	public static void RemoveHooks() {
		On.Celeste.Level.Update -= On_LevelUpdate_Build;
		Everest.Events.LevelLoader.OnLoadingThread -= OnLoadingThread_AddCursorDisplay;
	}
}

public sealed class BuildCursorDisplay : Entity {
	public BuildCursorDisplay() {
		Tag = Tags.HUD | Tags.Global;
		Depth = -0xabcdef;
	}

	public override void Render() {
		base.Render();
		if (Scene is not Level level || level.FrozenOrPaused || (level.Tracker.CountEntities<BuildController>() == 0 && !YaoiHelperModule.Settings.BuildAnywhere)) return;
		Vector2 cursorPos = new Vector2(BuildHandler.mouse_pos.X - (BuildHandler.mouse_pos.X % 8), BuildHandler.mouse_pos.Y - (BuildHandler.mouse_pos.Y % 8)) + level.LevelOffset;
		Color cursorColor = BuildHandler.valid_position switch {
			false => Color.Red,
			true when BuildHandler.building || BuildHandler.mining => Color.Yellow,
			_ => Color.LightGreen
		};

		for (int i = 0; i < 6; i++) {
			Draw.HollowRect(level.WorldToScreen(cursorPos) + new Vector2(i, i), 8*6 - 2*i, 8*6 - 2*i, cursorColor);
		}

		if (!BuildHandler.Unlimited) {
			ActiveFont.Draw($"{BuildHandler.TilesLeft}/{BuildHandler.TileLimit}", level.WorldToScreen(cursorPos + new Vector2(8, 8)), Vector2.Zero, Vector2.One / 2, cursorColor);
		}
	}

}
