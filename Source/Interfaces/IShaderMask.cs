using System.Collections.Generic;

namespace Celeste.Mod.YaoiHelper.Interfaces;

public interface IShaderMask {
	List<string> MaskGroups { get; }
	void RenderMask();
}
