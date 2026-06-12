using System.Collections.Generic;
using Monocle;

namespace Celeste.Mod.YaoiHelper.Interfaces;

public interface IShaderMask {
	List<string> MaskGroups { get; }
	void RenderMask();
}
