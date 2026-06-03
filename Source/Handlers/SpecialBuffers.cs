using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using MonoMod.Cil;

namespace Celeste.Mod.YaoiHelper.Handlers;

// TODO i have like no clue where to put this
[Submodule]
public static class SpecialBuffers {
	internal static void ApplyHooks() {
		IL.Celeste.Level.Render += IL_LevelRender_RenderToSpecialBuffers;
		IL.Celeste.LightingRenderer.BeforeRender += IL_LightingRendererBeforeRender_RenderWithoutBlur;
		On.Celeste.Level.Begin += On_LevelBegin_InitSpecialBuffers;
		On.Celeste.Level.End += On_LevelEnd_UnloadSpecialBuffers;
	}

	internal static void RemoveHooks() {
		IL.Celeste.Level.Render -= IL_LevelRender_RenderToSpecialBuffers;
		IL.Celeste.LightingRenderer.BeforeRender -= IL_LightingRendererBeforeRender_RenderWithoutBlur;
		On.Celeste.Level.Begin -= On_LevelBegin_InitSpecialBuffers;
		On.Celeste.Level.End -= On_LevelEnd_UnloadSpecialBuffers;
	}

	public static void On_LevelBegin_InitSpecialBuffers(On.Celeste.Level.orig_Begin orig, Level self) {
		orig(self);
		Init();
	}

	public static void On_LevelEnd_UnloadSpecialBuffers(On.Celeste.Level.orig_End orig, Level self) {
		orig(self);
		Unload();
	}

	internal static void IL_LightingRendererBeforeRender_RenderWithoutBlur(ILContext il) {
		ILCursor cursor = new ILCursor(il);

		cursor.GotoNext(MoveType.Before, cursor => cursor.MatchCallOrCallvirt(typeof(GaussianBlur).GetMethod("Blur")!));
		cursor.EmitLdsfld(typeof(GameplayBuffers).GetField("Light")!);
		cursor.EmitDelegate(renderLightWithoutBlur);
	}

	private static void renderLightWithoutBlur(VirtualRenderTarget source) {
		Engine.Graphics.GraphicsDevice.SetRenderTarget(Get("light_no_blur"));
		Engine.Graphics.GraphicsDevice.Clear(Color.Transparent);
		Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Matrix.Identity);
		Draw.SpriteBatch.Draw((Texture2D)source, Vector2.Zero, Color.White);
		Draw.SpriteBatch.End();
	}

	internal static void IL_LevelRender_RenderToSpecialBuffers(ILContext il) {
		ILCursor cursor = new ILCursor(il);

		cursor.GotoNext(MoveType.Before,
			cursor => cursor.MatchLdnull(), 
			cursor => cursor.MatchCallvirt<GraphicsDevice>("SetRenderTarget")
		);
		cursor.Index -= 2;

		// todo clean this up
		cursor.MoveAfterLabels();
		cursor.EmitLdarg0();
		cursor.EmitDelegate(renderToSpecialBuffers);
	}

	private static void renderToSpecialBuffers(Level level) {
		Engine.Graphics.GraphicsDevice.SetRenderTarget(Get("player"));
		Engine.Graphics.GraphicsDevice.Clear(Color.Transparent);
		Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, level.Camera.Matrix);
		if (level.Tracker.CountEntities<Player>() > 0) {
			foreach (Player player in level.Tracker.GetEntities<Player>().Cast<Player>()) {
				if (player.Visible) {
					player.Render();
				}
			}
		} else {
			foreach (PlayerDeadBody body in level.Entities.FindAll<PlayerDeadBody>()) {
				if (body.Visible) {
					body.Render();
				}
			}
		}
		Draw.SpriteBatch.End();

		// if (Engine.Commands.Open) {
		// 	level.Entities.DebugRender(level.Camera);
		// }

		Engine.Graphics.GraphicsDevice.SetRenderTarget(Get("particles"));
		Engine.Graphics.GraphicsDevice.Clear(Color.Transparent);
		Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, level.Camera.Matrix);
		level.ParticlesFG.Render();
		level.Particles.Render();
		level.ParticlesBG.Render();

		Draw.SpriteBatch.End();
	}

	private static readonly Dictionary<string, VirtualRenderTarget?> targets = [];

	public static VirtualRenderTarget? Get(string name) {
		return targets[name];
	}

	public static void Create(string name, int width, int height) {
		targets.Add(name, VirtualContent.CreateRenderTarget($"hd-shader-special-target-{name}", width, height));
	}	

	public static void Init() {
		Create("empty", 320, 180);
		Create("player", 320, 180);
		Create("particles", 320, 180);
		Create("light_no_blur", 320, 180);
		// Create("last_frame", 1920, 1080);
	}

	public static void Unload() {
		foreach (string target in targets.Keys) {
			targets[target]?.Dispose();
		}

		targets.Clear();
	}
}

