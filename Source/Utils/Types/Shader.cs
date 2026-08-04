using Microsoft.Xna.Framework.Graphics;

namespace Crackerberries.YaoiHelper.Types;

public record Shader(Effect Effect, string[] Textures, string? Target, int Priority);
