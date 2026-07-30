using System.Collections.Generic;

namespace Crackerberries.YaoiHelper.Interfaces;

public interface IShaderMask {
	List<string> MaskGroups { get; }
	bool LowRes { get; }
	void RenderMask();
}
