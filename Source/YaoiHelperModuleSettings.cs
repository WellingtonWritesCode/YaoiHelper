namespace Celeste.Mod.YaoiHelper;

public sealed class YaoiHelperModuleSettings : EverestModuleSettings {
	[SettingSubText("Requires a room reload to apply")]
	public bool BuildAnywhere { get; set; }
}
