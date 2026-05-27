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
	private static Vector2 mouse_pos;
	private static bool building;
	private static bool mining;
	private static bool valid_position;

	private static readonly char selectedTile = '3';

	public static void On_LevelUpdate_Build(On.Celeste.Level.orig_Update orig, Level level) {
		if (level.FrozenOrPaused || (level.Tracker.CountEntities<BuildController>() == 0 && !YaoiHelperModule.Settings.BuildAnywhere)) {
			orig(level);
			return;
		}

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

		if (!(building || mining) || !valid_position) {
			orig(level);
			return;
		};


		if (building) {
			if (level.SolidsData[tile.X, tile.Y] == '0')  {
				level.SolidTiles.Grid[tile.X, tile.Y] = true;
				level.SolidsData[tile.X, tile.Y] = selectedTile;
				UpdateTilesAround(level, tile, 2);
			}
		} else { // mining
			if (level.SolidsData[tile.X, tile.Y] != '0') {
				level.SolidTiles.Grid[tile.X, tile.Y] = false;
				level.SolidsData[tile.X, tile.Y] = '0';
				UpdateTilesAround(level, tile, 2);
			}
		}
		
		orig(level);
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

	private static void RenderBuildCursor(Scene scene) {
		if (scene is not Level level || level.FrozenOrPaused || (level.Tracker.CountEntities<BuildController>() == 0 && !YaoiHelperModule.Settings.BuildAnywhere)) return;
		Draw.HollowRect(new Vector2(mouse_pos.X - (mouse_pos.X % 8), mouse_pos.Y - (mouse_pos.Y % 8)) + level.LevelOffset, 8, 8, valid_position switch {
			false => Color.Red,
			true when building || mining => Color.Yellow,
			_ => Color.LightGreen
		});
	}

	public static void IL_GameplayRendererRender_RenderBuildCursor(ILContext il) {
		ILCursor cursor = new ILCursor(il);

		cursor.GotoNext(MoveType.After, cursor => cursor.MatchOr());
		cursor.Index++;
		cursor.EmitLdarg1();
		cursor.EmitDelegate(RenderBuildCursor);

	}

	public static void ApplyHooks() {
		On.Celeste.Level.Update += On_LevelUpdate_Build;
		IL.Celeste.GameplayRenderer.Render += IL_GameplayRendererRender_RenderBuildCursor;
	}

	public static void RemoveHooks() {
		On.Celeste.Level.Update -= On_LevelUpdate_Build;
		IL.Celeste.GameplayRenderer.Render -= IL_GameplayRendererRender_RenderBuildCursor;
	}
}
