using System.Collections.Generic;
using System.Linq;
using Celeste.Mod.YaoiHelper.Entities;
using Celeste.Mod.YaoiHelper.Triggers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Monocle;

namespace Celeste.Mod.YaoiHelper.Handlers;

[Submodule]
public static class BuildHandler {
	public static Vector2 MousePos { get; private set; }
	public static bool Building { get; private set; }
	public static bool Mining { get; private set; }
	public static bool IsValidPosition { get; private set; }

	private static readonly char selectedTile = '3';
	private static Dictionary<string, Dictionary<Point, TileModification>> modifications = [];
	
	private static int tileLimit = -1;
	private static bool unlimited = true;

	public static int TileLimit { get => tileLimit; set => tileLimit = value; }
	public static bool Unlimited { get => unlimited || YaoiHelperModule.Settings.BuildAnywhere; set => unlimited = value; }

	public static int TilesLeft(string level) => TileLimit - modifications[level].Count(x => x.Value.Type == ModificationType.Built);
	public static bool BuildRoom(string level) => modifications.ContainsKey(level);

	internal static void ApplyHooks() {
		On.Celeste.Level.Update += On_LevelUpdate_Build;
		Everest.Events.LevelLoader.OnLoadingThread += OnLoadingThread_AddCursorDisplayAndClearBuilds;
	}

	internal static void RemoveHooks() {
		On.Celeste.Level.Update -= On_LevelUpdate_Build;
		Everest.Events.LevelLoader.OnLoadingThread -= OnLoadingThread_AddCursorDisplayAndClearBuilds;
	}

	public static void Reset() {
		modifications = [];
	}

	public static void Reset(string level) {
		modifications[level] = [];
	}

	internal static void On_LevelUpdate_Build(On.Celeste.Level.orig_Update orig, Level level) {
		orig(level);

		if (level.FrozenOrPaused || (level.Tracker.CountEntities<BuildController>() == 0 && !YaoiHelperModule.Settings.BuildAnywhere)) return; 

		if (!modifications.ContainsKey(level.Session.Level)) {
			modifications[level.Session.Level] = [];
		}

		MouseState state = MInput.Mouse.CurrentState;
		MousePos = level.ScreenToWorld(new Vector2(MInput.Mouse.X - Engine.Viewport.X, MInput.Mouse.Y - Engine.Viewport.Y)) - level.LevelOffset;
		Point tile = new Point((int)MousePos.X / 8, (int)MousePos.Y / 8) + level.LevelSolidOffset;

		Building = state.LeftButton.HasFlag(ButtonState.Pressed);
		Mining = state.RightButton.HasFlag(ButtonState.Pressed);


		if (level.Tracker.CountEntities<BuildRegion>() == 0 || YaoiHelperModule.Settings.BuildAnywhere) {
			IsValidPosition = true;
		} else {
			IsValidPosition = false;
			foreach (BuildRegion buildRegion in level.Tracker.GetEntities<BuildRegion>().Cast<BuildRegion>()) {
				IsValidPosition = IsValidPosition || (buildRegion.Collider as Hitbox).Collide(MousePos + level.LevelOffset);
			}

			if (IsValidPosition && level.Tracker.GetEntity<Player>() is Player player) {
				foreach (BuildRegion buildRegion in level.Tracker.GetEntities<BuildRegion>().Cast<BuildRegion>().Where(x => x.PreventBuildingWhenInside)) {
					IsValidPosition = IsValidPosition && !(player.Collider as Hitbox).Collide(buildRegion.Collider);
				}
			}
		}

		if (!(Building || Mining) || !IsValidPosition) return;

		if (Building) {
			if (level.SolidsData[tile.X, tile.Y] == '0' && ((TilesLeft(level.Session.Level) > 0) || Unlimited))  {
				if (modifications[level.Session.Level].TryGetValue(tile, out TileModification modification) && modification.Type == ModificationType.Mined) {
					modifications[level.Session.Level].Remove(tile);
				} else {
					modifications[level.Session.Level].Add(tile, new TileModification {
						Type = ModificationType.Built,
						OrigTile = '0'
					});
				}

				level.SolidTiles.Grid[tile.X, tile.Y] = true;
				level.SolidsData[tile.X, tile.Y] = selectedTile;
				UpdateTilesAround(level, tile, 2);
			}
		} else { // mining
			if (level.SolidsData[tile.X, tile.Y] != '0') {
				if (modifications[level.Session.Level].TryGetValue(tile, out TileModification modification) && modification.Type == ModificationType.Built) {
					modifications[level.Session.Level].Remove(tile);
				} else {
					modifications[level.Session.Level].Add(tile, new TileModification {
						Type = ModificationType.Mined,
						OrigTile = level.SolidsData[tile.X, tile.Y]
					});
				}

				level.SolidTiles.Grid[tile.X, tile.Y] = false;
				level.SolidsData[tile.X, tile.Y] = '0';
				UpdateTilesAround(level, tile, 2);
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

	internal static void OnLoadingThread_AddCursorDisplayAndClearBuilds(Level level) {
        Reset(level.Session.Level);
		level.Add(new BuildCursorDisplay());
	}
}

internal struct TileModification {
	public ModificationType Type;
	public char OrigTile;
}

internal enum ModificationType {
	Built,
	Mined
}

public sealed class BuildCursorDisplay : Entity {
	public BuildCursorDisplay() {
		Tag = Tags.HUD | Tags.Global;
		Depth = -0xabcdef;
	}

	public override void Render() {
		base.Render();
		if (Scene is not Level level || level.FrozenOrPaused || (level.Tracker.CountEntities<BuildController>() == 0 && !YaoiHelperModule.Settings.BuildAnywhere) || !BuildHandler.BuildRoom(level.Session.Level)) return;
		Vector2 cursorPos = new Vector2(BuildHandler.MousePos.X - (BuildHandler.MousePos.X % 8), BuildHandler.MousePos.Y - (BuildHandler.MousePos.Y % 8)) + level.LevelOffset;
		Color cursorColor = BuildHandler.IsValidPosition switch {
			false => Color.Red,
			true when BuildHandler.Building || BuildHandler.Mining => Color.Yellow,
			_ => Color.LightGreen,
		};

		for (int i = 0; i < 6; i++) {
			Draw.HollowRect(level.WorldToScreen(cursorPos) + new Vector2(i, i), 8*6 - 2*i, 8*6 - 2*i, cursorColor);
		}

		if (!BuildHandler.Unlimited) {
			ActiveFont.Draw($"{BuildHandler.TilesLeft(level.Session.Level)}/{BuildHandler.TileLimit}", level.WorldToScreen(cursorPos + new Vector2(8, 8)), Vector2.Zero, Vector2.One / 2, cursorColor);
		}
	}

}
