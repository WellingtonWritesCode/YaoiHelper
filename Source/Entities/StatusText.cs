using Microsoft.Xna.Framework;
using Monocle;
using System.Text.RegularExpressions;

namespace Celeste.Mod.YaoiHelper.Entities;

internal sealed class StatusText : Entity {
	public static readonly Vector2 Padding = new Vector2(25f, 12.5f);

	public string Text;
	public float Size;
	public Color TextColor;

	public bool ShouldRender = true;

	public StatusText(string text, float size, Color color, bool top = false, float extraHPad = 0f, float extraVPad = 0f) {
		Text = text;
		Size = size;
		TextColor = color;
		Vector2 textSize = ActiveFont.Measure(text) * Size;
		if (top)
			Position = Padding + new Vector2(extraHPad, extraVPad);
		else
			Position = new Vector2(Padding.X + extraHPad, Engine.Height - textSize.Y - (Padding.Y + extraVPad));
		Tag = Tags.HUD | Tags.Global | Tags.FrozenUpdate | Tags.PauseUpdate | Tags.TransitionUpdate;
	}

	public override void Render() {
		if (ShouldRender)
			drawColored(Text, Position, Vector2.Zero, Vector2.One * Size, TextColor, 2, Color.Black);
	}

	private static void drawColored(string text, Vector2 position, Vector2 justify, Vector2 scale, Color color, float stroke, Color strokeColor) {
		if (!text.Contains('{')) {
			ActiveFont.DrawOutline(text, position, justify, scale, color, stroke, strokeColor);
			return;
		}
		Color newColor = color;
		Vector2 newPos = position;
		string[] segments = Regex.Split(text, @"([{}])", RegexOptions.NonBacktracking);
		for (int i = 0; i < segments.Length; i++) {
			if (segments[i] == "{") {
				string newColorHex = segments[++i];
				newColor = (newColorHex == "#") ? color : Calc.HexToColor(newColorHex);
				i += 2; // skip over }
			}

			// this being false can happen if for some reason we have two color changes in a row
			if (segments[i] != "{") {
				ActiveFont.DrawOutline(segments[i], newPos, justify, scale, newColor, stroke, strokeColor);
				newPos.X += ActiveFont.Measure(segments[i]).X * scale.X;
			}
		}
	}
}
