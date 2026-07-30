using System;

namespace Crackerberries.YaoiHelper;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
internal sealed class BootstrapAttribute : Attribute {
	public int Order { get; init; } = 0;
}

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
internal sealed class SubmoduleAttribute : Attribute {
	public int Order { get; init; } = 0;
	public bool HasSRTSupport { get; init; } = false;
}
